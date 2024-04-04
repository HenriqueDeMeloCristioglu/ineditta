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
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Application.Usuarios.Entities;
using System.Text;
using MySqlConnector;
using Ineditta.Repository.Extensions;
using Ineditta.API.ViewModels.Cnaes.Requests;
using Ineditta.API.ViewModels.Cnaes.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.Cnaes.UseCases.Upsert;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/cnaes")]
    [ApiController]
    public class CnaeController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public CnaeController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<CnaeViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] CnaeRequest request)
        {
            var siglasSindicatosPatronais = new List<string>();
            var siglasSindicatosLaborais = new List<string>();

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                if (request.PorGrupoDoUsuario)
                {
                    var parametersPorUsuario = new List<MySqlParameter>();

                    var stringBuilder = new StringBuilder(@"select cct.* 
                                                                from classe_cnae cct
                                                                    where exists(select 1 from cliente_unidades cut
                                                                    inner join usuario_adm uat on uat.email_usuario = @email
                                                                    and json_contains(cut.cnae_unidade, concat('{{""id"":', cct.id_cnae,'}}'))
                                                                    where case when uat.nivel = @nivel then true
                                                                               else cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon
                                                                          end)");

                    parametersPorUsuario.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                    parametersPorUsuario.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));

                    var query = await _context.ClasseCnaes
                                    .FromSqlRaw(stringBuilder.ToString(), parametersPorUsuario.ToArray())
                                    .AsNoTracking()
                                    .Select(cct => new CnaeViewModel
                                    {
                                        Id = cct.Id,
                                        Divisao = cct.Divisao,
                                        Descricao = cct.DescricaoDivisao,
                                        Subclasse = cct.SubClasse,
                                        DescricaoSubClasse = cct.DescricaoSubClasse,
                                        Categoria = cct.Categoria
                                    }).ToDataTableResponseAsync(request);

                    return Ok(query);
                }

                var sql = new StringBuilder(@"select * from classe_cnae_vw vw where 1 = 1");

                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;


                if (request.CnaesIds != null && request.CnaesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.CnaesIds, "vw.id", parameters, ref parametersCount);
                }
                else if (request.ForceGetByIds && (request.CnaesIds == null || !request.CnaesIds.Any()))
                {
                    return NoContent();
                }


                if (request.SindicatosPatronaisId != null)
                {
                    var sqlSindicatosPatronais = new StringBuilder(@" and (select max(1) from base_territorialsindpatro btt 
						                                                   inner join sind_patr spt on btt.sind_patronal_id_sindp = spt.id_sindp 
					                                                       where btt.classe_cnae_idclasse_cnae = vw.id");

                    QueryBuilder.AppendListToQueryBuilder(sqlSindicatosPatronais, request.SindicatosPatronaisId, "spt.id_sindp", parameters, ref parametersCount);

                    sqlSindicatosPatronais.Append(" ) > 0");

                    sql.Append(sqlSindicatosPatronais);

                    siglasSindicatosPatronais = _context.SindPatrs.Where(spt => request.SindicatosPatronaisId.Contains(spt.Id)).Select(spt => spt.Sigla).ToList();
                }

                if (request.SindicatosLaboraisId != null)
                {
                    var sqlSindicatosLaborais = new StringBuilder(@" and (select max(1)
	                                                                from base_territorialsindemp btet
	                                                                inner join sind_emp sett on btet.sind_empregados_id_sinde1  = sett.id_sinde  
	                                                                where btet.classe_cnae_idclasse_cnae  = vw.id");

                    QueryBuilder.AppendListToQueryBuilder(sqlSindicatosLaborais, request.SindicatosLaboraisId, "sett.id_sinde", parameters, ref parametersCount);

                    sqlSindicatosLaborais.Append(" ) > 0");

                    sql.Append(sqlSindicatosLaborais);

                    siglasSindicatosLaborais = _context.SindEmps.Where(sett => request.SindicatosLaboraisId.Contains(sett.Id)).Select(spt => spt.Sigla).ToList();
                }

                return Ok(await _context.ClassesCnaesVw
                           .FromSqlRaw(sql.ToString(), parameters.ToArray())
                           .AsNoTracking()
                           .Select(cc => new CnaeViewModel
                           {
                               Id = cc.Id,
                               Divisao = cc.Divisao,
                               Subclasse = cc.Subclasse,
                               Descricao = cc.DescricaoSubclasse,
                               Categoria = cc.Categoria,
                               SiglasSindicatosLaborais = cc.SiglasSindicatosLaborais,
                               SiglasSindicatosPatronais = cc.SiglasSindicatosPatronais,
                               SiglasSindicatosLaboraisFiltro = siglasSindicatosLaborais,
                               SiglasSindicatosPatronaisFiltro = siglasSindicatosPatronais

                           })
                           .ToDataTableResponseAsync(request));
            }

            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = _context.ClasseCnaes.AsNoTracking().Select(cct => new CnaeViewModel
                {
                    Id = cct.Id,
                    Divisao = cct.Divisao,
                    Descricao = cct.DescricaoDivisao,
                    Subclasse = cct.SubClasse,
                    DescricaoSubClasse = cct.DescricaoSubClasse,
                    Categoria = cct.Categoria
                });

                if (request.PorGrupoDoUsuario)
                {
                    var parameters = new List<MySqlParameter>();

                    var stringBuilder = new StringBuilder(@"select cct.* 
                                                                from classe_cnae cct
                                                                    where exists(select 1 from cliente_unidades cut
                                                                    inner join usuario_adm uat on uat.email_usuario = @email
                                                                    and json_contains(cut.cnae_unidade, concat('{{""id"":', cct.id_cnae,'}}'))
                                                                    where case when uat.nivel = @nivel then true
                                                                               else cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon
                                                                          end)");

                    parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                    parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                    parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                    query = _context.ClasseCnaes
                                    .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                                    .AsNoTracking()
                                    .Select(cct => new CnaeViewModel
                                    {
                                        Id = cct.Id,
                                        Divisao = cct.Divisao,
                                        Descricao = cct.DescricaoDivisao,
                                        Subclasse = cct.SubClasse,
                                        DescricaoSubClasse = cct.DescricaoSubClasse,
                                        Categoria = cct.Categoria
                                    });
                }

                if (request.PorUsuario)
                {
                    var parameters = new List<MySqlParameter>();
                    int parametersCount = 0;

                    var stringBuilder = new StringBuilder(@"select cct.* 
                                                                from classe_cnae cct
                                                                    where exists(select 1 from cliente_unidades cut
                                                                    inner join usuario_adm uat on uat.email_usuario = @email
                                                                    and json_contains(cut.cnae_unidade, concat('{{""id"":', cct.id_cnae,'}}'))
                                                                    where case when uat.nivel = @nivel then true
                                                                               when uat.nivel = @nivelGrupoEconomico then cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon
                                                                                else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) 
                                                                          end");

                    parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                    parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                    parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));


                    if (request.GrupoEconomicoId != null && request.GrupoEconomicoId > 0)
                    {
                        stringBuilder.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                        parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
                    }

                    if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
                    }

                    if (request.MatrizesIds != null && request.MatrizesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
                    }

                    if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
                    }

                    if (request.EstabelecimentosIds != null && request.EstabelecimentosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.EstabelecimentosIds, "cut.id_unidade", parameters, ref parametersCount);
                    }

                    stringBuilder.Append(')');

                    if (request.DistinctDivisao)
                    {
                        stringBuilder.Append(@" group by cct.divisao_cnae ");
                    }

                    query = _context.ClasseCnaes
                                    .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                                    .AsNoTracking()
                                    .Select(cct => new CnaeViewModel
                                    {
                                        Id = cct.Id,
                                        Divisao = cct.Divisao,
                                        Descricao = cct.DescricaoDivisao,
                                        Subclasse = cct.SubClasse,
                                        DescricaoSubClasse = cct.DescricaoSubClasse,
                                        Categoria = cct.Categoria
                                    });
                }

                return !string.IsNullOrEmpty(request.Columns)
                ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
                : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet("inclusao")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<CnaeViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosInclusaoAsync([FromQuery] CnaeRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            return Ok(await _context.ClasseCnaes
                           .Select(cc => new CnaesDataTableViewModel
                           {
                               Id = cc.Id,
                               Categoria = cc.Categoria,
                               DescricaoDivisao = cc.DescricaoDivisao,
                               DescricaoSubclasse = cc.DescricaoSubClasse,
                               Divisao = cc.Divisao,
                               Subclasse = cc.SubClasse
                           })
                           .ToDataTableResponseAsync(request));
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(CnaePorIdViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosInclusaoAsync([FromRoute] int id)
        {
            if (Request.Headers.Accept == MediaTypeNames.Application.Json)
            {
                return NotAcceptable();
            }

            var result = await _context.ClasseCnaes
                           .Where(c => c.Id == id)
                           .Select(cc => new CnaePorIdViewModel
                           {
                               Id = cc.Id,
                               Categoria = cc.Categoria,
                               DescricaoDivisao = cc.DescricaoDivisao,
                               DescricaoSubclasse = cc.DescricaoSubClasse,
                               Divisao = cc.Divisao,
                               Subclasse = cc.SubClasse
                           })
                           .SingleOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpGet("select")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodos([FromQuery] CnaeRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NoContent();
            }

            FormattableString query = $@"select cc.* from ineditta.classe_cnae cc 
                            inner join lateral(
                            select divisao_cnae, max(id_cnae) maior_id
                            from ineditta.classe_cnae
                            group by 1) cct 
                            where cct.maior_id = cc.id_cnae";

            return Ok(await (_context.ClasseCnaes.FromSql(query).Select(cct => new OptionModel<int>
            {
                Id = cct.Divisao,
                Description = cct.DescricaoDivisao
            }).ToListAsync()));
        }


        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<IActionResult> Criar([FromBody] UpsertCnaeRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertCnaeRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }
    }
}
