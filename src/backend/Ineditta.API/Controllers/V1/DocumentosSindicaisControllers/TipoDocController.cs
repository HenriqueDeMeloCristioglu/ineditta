using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.Application.TiposDocumentos.UseCases.Upsert;
using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.API.ViewModels.TiposDocumentos.Requests;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.TiposDocumentos.ViewModels;

namespace Ineditta.API.Controllers.V1.DocumentosSindicaisControllers
{
    [Route("v{version:apiVersion}/tipo-docs")]
    [ApiController]
    public class TipoDocController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IMediator _mediator;

        public TipoDocController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(TipoDocumentoDataTableViewModel), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<TipoDocumentoViewModel>), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] TipoDocumentoRequest request, [FromQuery] DataTableRequest requestDataTable)
        {
            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return Ok(await _context.TipoDocs
                            .AsNoTracking()
                            .Where(dc => dc.Processado == (request.Processado ?? false) && (request.TiposDocumentosIds == null || request.TiposDocumentosIds!.Contains(dc.Grupo)))
                            .Select(dc => new TipoDocumentoViewModel
                            {
                                Id = dc.Id,
                                Nome = dc.Nome,
                                Tipo = dc.Grupo,
                                Processado = dc.Processado
                            })
                            .OrderBy(x => x.Nome)
                            .ToListAsync());
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from td in _context.TipoDocs
                                    select new TipoDocumentoDataTableViewModel
                                    {
                                        Id = td.Id,
                                        Nome = td.Nome,
                                        Tipo = td.Grupo,
                                        Sigla = td.Sigla
                                    }).ToDataTableResponseAsync(requestDataTable);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (Request.Headers.Accept == SelectMediaType.ContentType && request.FiltrarSelectType)
            {
                return Ok(
                        await _context.TipoDocs
                            .AsNoTracking()
                            .Where(dc => dc.Processado == (request.Processado ?? false))
                            .Select(dc => new OptionModel<int>
                            {
                                Id = dc.Id,
                                Description = dc.Nome
                            })
                            .ToListAsync()
                        );

            }

            return Request.Headers.Accept == SelectMediaType.ContentType
                ? Ok(await _context.TipoDocs
                            .AsNoTracking()
                            .Select(dc => new OptionModel<int>
                            {
                                Id = dc.Id,
                                Description = dc.Nome
                            })
                            .OrderBy(x => x.Description)
                            .ToListAsync())
                    : NotAcceptable();
        }

        [HttpGet("tipos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTiposAsync()
        {
            return Request.Headers.Accept == SelectMediaType.ContentType ?
                Ok(await _context.TipoDocs
                            .AsNoTracking()
                            .Select(dc => dc.Grupo)
                            .Distinct()
                            .Select(tipoDocumento => new OptionModel<string>
                            {
                                Id = tipoDocumento,
                                Description = tipoDocumento
                            }).ToListAsync())
                : NotAcceptable();
        }

        [HttpGet("{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(TipoDocumentoVerPorIdViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterAcompanhamentoPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from td in _context.TipoDocs
                                where td.Id == id
                                select new TipoDocumentoVerPorIdViewModel
                                {
                                    Id = td.Id,
                                    Nome = td.Nome,
                                    Tipo = td.Grupo,
                                    Sigla = td.Sigla,
                                    Modulo = td.ModuloCadastro == TipoModuloCadastro.Processado ? "Processado" : "Geral",
                                    Processado = td.Processado ? "S" : "N",
                                }).SingleOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertTipoDocumentoRequest request)
        {
            var insertResult = await _mediator.Send(request);
            if (insertResult.IsFailure) return FromResult(insertResult);

            return CreatedResult(insertResult.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> AtualizarAsync(int id, [FromBody] UpsertTipoDocumentoRequest request)
        {
            request.Id = id;
            return await Dispatch(request);
        }
    }
}
