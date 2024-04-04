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
using Ineditta.API.ViewModels.BasesTerritoriais.SindicatosPatronais;
using Ineditta.Repository.Models;

namespace Ineditta.API.Controllers.V1.BasesTerritoriais
{
    [Route("v{version:apiVersion}/bases-territoriais-sindicatos-patronais")]
    [ApiController]
    public class BaseTerritorialSindicatoPatronalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public BaseTerritorialSindicatoPatronalController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<BaseTerritorialSindicatoPatronalViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] BaseTerritorialSindicatoPatronalRequest request)
        {
#pragma warning disable CA1305
            if (request.ApenasVigentes && Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return Ok(await (from bts in _context.BaseTerritorialsindpatros
                                 join spt in _context.SindPatrs on bts.SindicatoId equals spt.Id
                                 join lct in _context.Localizacoes on bts.LocalizacaoId equals lct.Id
                                 join cnc in _context.ClasseCnaes on bts.CnaeId equals cnc.Id
                                 where bts.SindicatoId == request.SindicatoPatronalId
                                 && (bts.DataFinal == null || bts.DataFinal < DateOnly.FromDateTime(DateTime.MinValue))
                                 select new BaseTerritorialSindicatoPatronalViewModel
                                 {
                                     Id = bts.Id,
                                     DataFinal = bts.DataFinal,
                                     DataInicial = bts.DataInicial,
                                     Sigla = spt.Sigla,
                                     DescricaoSubClasse = cnc.DescricaoSubClasse,
                                     LocalizacaoId = lct.Id,
                                     Pais = lct.Pais.Id,
                                     Regiao = lct.Regiao.Id,
                                     Estado = lct.Estado.Id,
                                     CodigoUf = lct.Uf.Id,
                                     Municipio = lct.Municipio,
                                     CnaeId = cnc.Id,
                                     SubclasseCnae = cnc.SubClasse.ToString(),
                                     DescricaoCnae = cnc.DescricaoSubClasse
                                 })
                            .ToDataTableResponseAsync(request));
            }

            return Request.Headers.Accept != DatatableMediaType.ContentType
                ? NotAcceptable()
                : Ok(await (from bts in _context.BaseTerritorialsindpatros
                            join spt in _context.SindPatrs on bts.SindicatoId equals spt.Id
                            join lct in _context.Localizacoes on bts.LocalizacaoId equals lct.Id
                            join cnc in _context.ClasseCnaes on bts.CnaeId equals cnc.Id
                            where bts.SindicatoId == request.SindicatoPatronalId
                            && (request.DataFinal == null || bts.DataFinal == request.DataFinal)
                            select new BaseTerritorialSindicatoPatronalViewModel
                            {
                                Id = bts.Id,
                                DataFinal = bts.DataFinal,
                                DataInicial = bts.DataInicial,
                                Sigla = spt.Sigla,
                                DescricaoSubClasse = cnc.DescricaoSubClasse,
                                LocalizacaoId = lct.Id,
                                Pais = lct.Pais.Id,
                                Regiao = lct.Regiao.Id,
                                Estado = lct.Estado.Id,
                                CodigoUf = lct.Uf.Id,
                                Municipio = lct.Municipio,
                                CnaeId = cnc.Id,
                                SubclasseCnae = cnc.SubClasse.ToString(),
                                DescricaoCnae = cnc.DescricaoSubClasse
                            })
                            .ToDataTableResponseAsync(request));
#pragma warning restore CA1305
        }
    }


}
