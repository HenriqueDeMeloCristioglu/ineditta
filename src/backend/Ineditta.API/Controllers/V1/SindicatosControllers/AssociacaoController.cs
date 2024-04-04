using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using MediatR;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.API.ViewModels.Associacoes.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/associacoes")]
    [ApiController]
    public class AssociacaoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private const string Confederacao = "confederação";
        private const string Federacao = "federação";

        public AssociacaoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AssociacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Associacoes
                            .AsNoTracking()
                            .Select(cct => new AssociacaoViewModel
                            {
                                Id = cct.IdAssociacao,
                                Area = cct.AreaGeoeconomica,
                                Cnpj = cct.Cnpj,
                                Grau = cct.Grau,
                                Grupo = cct.Grupo,
                                Sigla = cct.Sigla,
                                Telefone = cct.Telefone
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AssociacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [Route("confederacoes")]
        public async ValueTask<IActionResult> ObterConfederacoesAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Associacoes
                            .AsNoTracking()
                            .Where(cct => cct.Grau == Confederacao)
                            .Select(cct => new AssociacaoViewModel
                            {
                                Id = cct.IdAssociacao,
                                Area = cct.AreaGeoeconomica,
                                Cnpj = cct.Cnpj,
                                Grau = cct.Grau,
                                Grupo = cct.Grupo,
                                Sigla = cct.Sigla,
                                Telefone = cct.Telefone
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AssociacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [Route("federacoes")]
        public async ValueTask<IActionResult> ObterFederacoesAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Associacoes
                            .AsNoTracking()
                            .Where(cct => cct.Grau == Federacao)
                            .Select(cct => new AssociacaoViewModel
                            {
                                Id = cct.IdAssociacao,
                                Area = cct.AreaGeoeconomica,
                                Cnpj = cct.Cnpj,
                                Grau = cct.Grau,
                                Grupo = cct.Grupo,
                                Sigla = cct.Sigla,
                                Telefone = cct.Telefone
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

    }
}
