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
using Ineditta.API.ViewModels.BasesTerritoriais.SindicatosLaborais.ViewModels;
using Ineditta.API.ViewModels.BasesTerritoriais.SindicatosLaborais.Requests;
using System.Text;
using Ineditta.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Ineditta.API.Controllers.V1.BasesTerritoriais
{
    [Route("v{version:apiVersion}/bases-territoriais-sindicatos-laborais")]
    [ApiController]
    public class BaseTerritorialSindicatoLaboralController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public BaseTerritorialSindicatoLaboralController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<BaseTerritorialSindicatoLaboralViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] BaseTerritorialSindicatoLaboralRequest request)
        {
#pragma warning disable CA1305
            if (request.ApenasVigentes && Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return Ok(await (from bts in _context.BaseTerritorialsindemps
                                 join spt in _context.SindEmps on bts.SindicatoId equals spt.Id
                                 join lct in _context.Localizacoes on bts.LocalizacaoId equals lct.Id
                                 join cnc in _context.ClasseCnaes on bts.CnaeId equals cnc.Id
                                 where bts.SindicatoId == request.SindicatoLaboralId
                                 && (bts.DataFinal == null || bts.DataFinal < DateOnly.FromDateTime(DateTime.MinValue))
                                 select new BaseTerritorialSindicatoLaboralViewModel
                                 {
                                     Id = bts.Id,
                                     DataFinal = bts.DataFinal,
                                     DataInicial = bts.DataInicial,
                                     DataNegociacao = bts.DataNegociacao,
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
                : Ok(await (from bts in _context.BaseTerritorialsindemps
                            join spt in _context.SindEmps on bts.SindicatoId equals spt.Id
                            join lct in _context.Localizacoes on bts.LocalizacaoId equals lct.Id
                            join cnc in _context.ClasseCnaes on bts.CnaeId equals cnc.Id
                            where bts.SindicatoId == request.SindicatoLaboralId
                            && (request.DataFinal == null || bts.DataFinal == request.DataFinal)
                            select new BaseTerritorialSindicatoLaboralViewModel
                            {
                                Id = bts.Id,
                                DataFinal = bts.DataFinal,
                                DataInicial = bts.DataInicial,
                                DataNegociacao = bts.DataNegociacao,
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

        [HttpGet]
        [Route("localizacoes")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<BaseTerritorialSindicatoLaboralViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosPorSindicatosAsync([FromQuery] BaseTerritorialSindicatoLaboralPorSindicatosRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var sql = new StringBuilder(@"SELECT DISTINCT * from base_territorial_sindicato_laboral_localizacao vw
                                        where true");

            if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosLaboraisIds, "vw.sindicato_laboral_id", parameters, ref parametersCount);
            }

            if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
            {
                sql.Append(@"and exists (
	                            select * from base_territorialsindpatro btspt
                                where true");

                QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosPatronaisIds, "btspt.sind_patronal_id_sindp", parameters, ref parametersCount);

                sql.Append(@"
	                            and btspt.localizacao_id_localizacao1 = l.id_localizacao 
                            )");
            }

            var result = await _context.BasesTerritoriaisLaboraisLocalizacoesVw.FromSqlRaw(sql.ToString(), parameters.ToArray())
                            .Select(b => new BaseTerritorialSindicatoLboralPorSindicatosViewModel
                            {
                                Id = b.Id,
                                Uf = b.Uf,
                                Municipio = b.Municipio,
                                Sigla = b.Sigla,
                                Pais = b.Pais
                            })
                            .ToDataTableResponseAsync(request);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
