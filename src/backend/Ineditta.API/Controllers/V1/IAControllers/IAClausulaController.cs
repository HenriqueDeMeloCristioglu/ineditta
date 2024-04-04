using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Ineditta.Application.AIs.Clausulas.Entities;
using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.Application.AIs.Clausulas.UseCases.Upsert;
using Ineditta.Application.AIs.Clausulas.UseCases.Delete;
using Ineditta.API.ViewModels.IAs.Clausulas;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1.IAControllers
{
    [Route("v{version:apiVersion}/ia-clausulas")]
    [ApiController]
    public class IAClausulaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public IAClausulaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }



        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async ValueTask<IActionResult> ObterTodosAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            List<OptionModel<int>> opcoes = new()
            {
                new OptionModel<int>
                {
                    Id = (int)IAClausulaStatus.Consistente,
                    Description = IAClausulaStatus.Consistente.ToString()
                },
                new OptionModel<int>
                {
                    Id = (int)IAClausulaStatus.Inconsistente,
                    Description = IAClausulaStatus.Inconsistente.ToString()
                }
            };

            return Ok(opcoes);
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IAClausula), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from iac in _context.IAClausulas
                                join ids in _context.IADocumentoSindical on iac.IADocumentoSindicalId equals ids.Id
                                join ec in _context.EstruturaClausula on iac.EstruturaClausulaId equals ec.Id into _ec
                                from ec in _ec.DefaultIfEmpty()
                                join ds in _context.DocSinds on ids.DocumentoReferenciaId equals ds.Id into _ds
                                from ds in _ds.DefaultIfEmpty()
                                join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                from td in _td.DefaultIfEmpty()
                                join sn in _context.Sinonimos on iac.SinonimoId equals sn.Id into _sn
                                from sn in _sn.DefaultIfEmpty()
                                select new IAClausulaViewModel
                                {
                                    Id = iac.Id,
                                    EstruturaClausulaId = iac.EstruturaClausulaId,
                                    EstruturaClausulaNome = ec.Nome,
                                    DocumentoSindicalId = iac.IADocumentoSindicalId,
                                    DocumentoSindicalNome = td.Nome,
                                    Numero = iac.Numero,
                                    SinonimoId = iac.SinonimoId,
                                    SinonimoNome = sn.Nome ?? string.Empty,
                                    Status = iac.Status,
                                    Texto = iac.Texto,
                                    GrupoInformacaoAdicional = (from infad in _context.EstruturaClausulasAdTipoinformacaoadicionals
                                                                join tp_if_ad in _context.AdTipoinformacaoadicionals on infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional equals tp_if_ad.Cdtipoinformacaoadicional into _tp_if_ad
                                                                from tp_if_ad in _tp_if_ad.DefaultIfEmpty()
                                                                where infad.EstruturaClausulaIdEstruturaclausula == ec.Id
                                                                select new ClausulaClienteGrupoInformacoesAdicionais
                                                                {
                                                                    TipoInformacaoId = tp_if_ad.Cdtipoinformacaoadicional,
                                                                    NomeTipoInformacao = tp_if_ad.Nmtipoinformacaoadicional ?? string.Empty,
                                                                    EstruturaId = ec.Id
                                                                })
                                                                .AsSingleQuery()
                                                                .FirstOrDefault()
                                })
                                .SingleOrDefaultAsync(c => c.Id == id);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertIAClausulaRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertIAClausulaRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            request.Id = id;

            return await Dispatch(request);
        }

        [HttpDelete("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status204NoContent, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> DeletarAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var request = new DeleteIAClausulaRequest
            {
                Id = id,
            };

            request.Id = id;

            return await Dispatch(request);
        }
    }
}
