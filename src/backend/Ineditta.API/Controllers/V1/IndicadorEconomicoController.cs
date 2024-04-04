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
using Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert;
using System.Text;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels;
using Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels.Home;
using Ineditta.API.ViewModels.IndicadoresEconomicos.Requests.Home;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/indicadores-economicos")]
    [ApiController]
    public class IndicadorEconomicoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public IndicadorEconomicoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpGet]
        [Route("principais")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<IndicadorEconomicoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosIndicadoresEconomicosAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.Indecons
                            .AsNoTracking()
                            .Select(ind => new IndicadorEconomicoViewModel
                            {
                                Id = ind.IdIndecon,
                                Origem = ind.Origem,
                                Fonte = ind.Fonte,
                                Indicador = ind.Indicador,
                                Data = ind.Data,
                                IdUsuario = ind.IdUsuario,
                                DadoProjetado = ind.DadoProjetado,
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

        [HttpGet]
        [Route("reais")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<IndicadorEconomicoRealViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosIndicadoresEconomicosReaisAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.IndeconReals
                            .AsNoTracking()
                            .Select(indr => new IndicadorEconomicoRealViewModel
                            {
                                Id = indr.Id,
                                DadoReal = indr.DadoReal,
                                Indicador = indr.Indicador,
                                PeriodoData = indr.PeriodoData,
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

        [HttpGet("principais/{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IndicadorEconomicoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPrincipalPorIdAsync([FromRoute] int id)
        {
            var result = await (from ind in _context.Indecons
                                select new IndicadorEconomicoViewModel
                                {
                                    Id = ind.IdIndecon,
                                    Origem = ind.Origem,
                                    Fonte = ind.Fonte,
                                    Indicador = ind.Indicador,
                                    Data = ind.Data,
                                    IdUsuario = ind.IdUsuario,
                                    DadoProjetado = ind.DadoProjetado,
                                }).SingleOrDefaultAsync(indw => indw.Id == id);

            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpGet("reais/{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IndicadorEconomicoRealViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterRealPorIdAsync([FromRoute] int id)
        {
            var result = await (from indr in _context.IndeconReals
                                select new IndicadorEconomicoRealViewModel
                                {
                                    Id = indr.Id,
                                    DadoReal = indr.DadoReal,
                                    Indicador = indr.Indicador,
                                    PeriodoData = indr.PeriodoData,
                                }).SingleOrDefaultAsync(indw => indw.Id == id);

            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpPost("principais")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirPrincipalAsync([FromBody] UpsertIndicadorEconomicoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("principais")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarPrincipalAsync([FromBody] UpsertIndicadorEconomicoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPost("reais")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirRealAsync([FromBody] UpsertIndicadorEconomicoRealRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("reais")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarRealAsync([FromBody] UpsertIndicadorEconomicoRealRequest request)
        {
            return await Dispatch(request);
        }

        [HttpGet("home")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<IndicadorEconomicoHomeViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]

        public async ValueTask<IActionResult> ObterIndicadoresHomeAsync([FromQuery] IndicadorEconomicoHomeRequest filtroHomeRequest)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@meses", filtroHomeRequest.NumeroMeses ?? 12 },
                { "@email", _userInfoService.GetEmail()! },
                { "@nivel", nameof(Nivel.Ineditta) }
            };

            var sql = new StringBuilder(@"select 1 tipo, 
                                          indicador, 
                                          periodo_data as periodo, 
                                          COALESCE(format(dado_real, 3), 0) indice,
                                          '' fonte
                                          from indecon_real 
                                          WHERE periodo_data BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL @meses MONTH) AND DATE_FORMAT(NOW(), '%Y-%m-01')
                                          group by 1, 2, 3
                                          union all
                                          select 2 tipo,  iet.indicador, 
	                                                STR_TO_DATE(iet.data, '%Y-%m-%d' ) as periodo,
	                                                IFNULL(FORMAT(max(iet.dado_projetado), 3), 0) indice,
	                                                max(fonte) fonte
                                          from indecon iet
                                          inner join usuario_adm uat on uat.email_usuario = @email
                                          WHERE iet.data BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-01'), INTERVAL @meses MONTH) AND DATE_FORMAT(NOW(), '%Y-%m-01')
                                          and case when uat.nivel = @nivel then iet.origem like '%Ineditta%' else iet.origem like '%Cliente%' and iet.cliente_grupo_id_grupo_economico = uat.id_grupoecon end
                                          group by 1, 2, 3");

            var result = await _context.SelectFromRawSqlAsync<IndicadorEconomicoHomeTipoViewModel>(sql.ToString(), parameters);

            return result is null ? NoContent() : Ok(IndicadorEconomicoHomeViewModel.Converter(result));
        }
    }
}
