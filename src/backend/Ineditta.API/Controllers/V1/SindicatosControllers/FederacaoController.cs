using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.Application.Federacoes.UseCases.Upsert;
using Ineditta.API.ViewModels.Federacoes.ViewModels;

namespace Ineditta.API.Controllers.V1.SindicatosControllers
{
    [ApiController]
    [Route("v{version:apiVersion}/federacoes")]
    public class FederacaoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public FederacaoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context)
            : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertFederacaoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertFederacaoRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<FederacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<FederacaoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Associacoes
                            .AsNoTracking()
                            .Select(fd => new FederacaoViewModel
                            {
                                Id = fd.IdAssociacao,
                                Sigla = fd.Sigla,
                                CNPJ = fd.Cnpj,
                                Telefone = fd.Telefone,
                                AreaGeoeconomica = fd.AreaGeoeconomica,
                                Grau = fd.Grau,
                                Grupo = fd.Grupo
                            })
                            .Where(p => p.Grau == "federação")
                            .ToDataTableResponseAsync(request))
                : Ok(await _context.Associacoes
                        .AsNoTracking()
                        .Select(fd => new FederacaoViewModel
                        {
                            Id = fd.IdAssociacao,
                            Sigla = fd.Sigla,
                            CNPJ = fd.Cnpj,
                            Telefone = fd.Telefone,
                            AreaGeoeconomica = fd.AreaGeoeconomica,
                            Grau = fd.Grau,
                            Grupo = fd.Grupo
                        })
                        .Where(p => p.Grau == "federação")
                        .ToListAsync());
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
