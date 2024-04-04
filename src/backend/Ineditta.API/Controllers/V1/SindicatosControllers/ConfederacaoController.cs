using System.Net.Mime;

using Ineditta.API.ViewModels.Confederacoes.ViewModels;
using Ineditta.Application.Confederacoes.UseCases.Upsert;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/confederacoes")]
    public class ConfederacaoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public ConfederacaoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context)
            : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertConfederacaoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertConfederacaoRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ConfederacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ConfederacaoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Associacoes
                            .AsNoTracking()
                            .Select(cfd => new ConfederacaoViewModel
                            {
                                Id = cfd.IdAssociacao,
                                Sigla = cfd.Sigla,
                                CNPJ = cfd.Cnpj,
                                Telefone = cfd.Telefone,
                                AreaGeoeconomica = cfd.AreaGeoeconomica,
                                Grau = cfd.Grau,
                                Grupo = cfd.Grupo
                            })
                            .Where(p => p.Grau == "confederação")
                            .ToDataTableResponseAsync(request))
                : Ok(await _context.Associacoes
                        .AsNoTracking()
                        .Select(cfd => new ConfederacaoViewModel
                        {
                            Id = cfd.IdAssociacao,
                            Sigla = cfd.Sigla,
                            CNPJ = cfd.Cnpj,
                            Telefone = cfd.Telefone,
                            AreaGeoeconomica = cfd.AreaGeoeconomica,
                            Grau = cfd.Grau,
                            Grupo = cfd.Grupo
                        })
                        .Where(p => p.Grau == "confederação")
                        .ToListAsync());
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
