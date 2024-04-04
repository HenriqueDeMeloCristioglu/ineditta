using System.Net.Mime;
using System.Text;

using DocumentFormat.OpenXml.Presentation;

using Ineditta.API.Builders.Sindicatos;
using Ineditta.API.Factories;
using Ineditta.API.Filters;
using Ineditta.API.ViewModels.ClientesUnidades.ViewModels;
using Ineditta.API.ViewModels.Sindicatos;
using Ineditta.API.ViewModels.Sindicatos.Dtos;
using Ineditta.API.ViewModels.Sindicatos.ViewModels;
using Ineditta.API.ViewModels.SindicatosLaborais.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/sindicatos")]
    [ApiController]
    public class SindicatoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly SindicatoBuilder _sindicatoBuilder;
        private readonly SindicatoLaboralBuilder _sindicatoLaboralBuilder;
        private readonly SindicatoPatronalBuilder _sindicatoPatronalBuilder;
        public SindicatoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, SindicatoBuilder sindicatoBuilder, SindicatoLaboralBuilder sindicatoLaboralBuilder, SindicatoPatronalBuilder sindicatoPatronalBuilder)
            : base(mediator, requestStateValidator)
        {
            _sindicatoBuilder = sindicatoBuilder;
            _sindicatoLaboralBuilder = sindicatoLaboralBuilder;
            _sindicatoPatronalBuilder = sindicatoPatronalBuilder;
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<SindicatosViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterSindicatosAsync([FromQuery] SindicatosRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 from cliente_unidades cut
	                                                            inner join base_territorialsindemp bemp on bemp.sind_empregados_id_sinde1 = sinde.id_sinde
	                                                            left join base_territorialsindpatro bpatr on bpatr.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1
                                                                and bpatr.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join localizacao loc on loc.id_localizacao = bemp.localizacao_id_localizacao1
	                                                            inner join classe_cnae cc on cc.id_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
	                                                            inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join usuario_adm uat on uat.email_usuario = @email
	                                                            where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bemp.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = 'Ineditta' then true 
                                                                        when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else json_contains(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                            from sind_patr sindp
                                                            where exists(select 1 from cliente_unidades cut
                                                                inner join base_territorialsindpatro bpatr on bpatr.sind_patronal_id_sindp = sindp.id_sindp
                                                                inner join localizacao loc on loc.id_localizacao = bpatr.localizacao_id_localizacao1
                                                                inner join classe_cnae cc on cc.id_cnae = bpatr.classe_cnae_idclasse_cnae
                                                                inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
                                                                inner join cnae_emp as cet on (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join base_territorialsindemp AS bemp 
                                                                    on (bemp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                    AND bemp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                inner join usuario_adm uat on uat.email_usuario = @email
                                                                where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bpatr.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                sindEmpregadosString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                sindPatronaisString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
                QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindLaboraisIds, "sinde.id_sinde", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindLaboraisIds, "bemp.sind_empregados_id_sinde1", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindPatronaisIds, "bpatr.sind_patronal_id_sindp", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindPatronaisIds, "sindp.id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindEmpregadosString.Append(')');
            sindPatronaisString.Append(')');

            var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(set => new SindicatoLaboralViewModel
                    {
                        Id = set.Id,
                        Cnpj = set.Cnpj.Value,
                        RazaoSocial = set.RazaoSocial ?? string.Empty,
                        Municipio = set.Municipio ?? string.Empty,
                        Uf = set.Uf ?? string.Empty,
                        Sigla = set.Sigla
                    });

            var querySindp = _context.SindPatrs
                .FromSqlRaw(sindPatronaisString.ToString(), parameters.ToArray())
                .AsNoTracking()
                .Select(spt => new SindicatoPatronalViewModel
                {
                    Id = spt.Id,
                    Sigla = spt.Sigla,
                    Cnpj = spt.Cnpj.Value ?? string.Empty,
                    RazaoSocial = spt.RazaoSocial ?? string.Empty,
                    Municipio = spt.Municipio ?? string.Empty,
                    Uf = spt.Uf ?? string.Empty,
                });

            var resultQuerySinde = await querySinde.ToListAsync();
            var resultQuerySindp = await querySindp.ToListAsync();

            var sindeIds = resultQuerySinde.Select(x => x.Id);
            var sindpIds = resultQuerySindp.Select(x => x.Id);

            /// MANDATOS START

            var parametrosMandatos = new List<MySqlParameter>();

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var resultEmps = await _context.SindDiremps.Where(x => sindeIds.Any(id => id == x.SindEmpIdSinde)).GroupBy(x => x.SindEmpIdSinde).Select(sd => sd.OrderByDescending(e => e.TerminoMandatoe).First().TerminoMandatoe).ToListAsync();

            var resultPatrs = await _context.SindDirpatros.Where(x => sindpIds.Any(id => id == x.SindicatoPatronalId)).GroupBy(x => x.SindicatoPatronalId).Select(sd => sd.OrderByDescending(e => e.DataFimMandato).First().DataFimMandato).ToListAsync();

            var resultFases = await (from acsl in _context.AcompanhamentoCctSinditoLaboral
                                     join cct in _context.AcompanhamentoCct on acsl.AcompanhamentoCctId equals cct.Id
                                     where sindeIds.Any(id => id == acsl.SindicatoId)
                                     select new
                                     {
                                         cct.FaseId
                                     }).ToListAsync();
            var negVencidas = 0;
            var negVigentes = 0;

            if (resultFases != null)
            {
                negVencidas = resultFases.Count(a => a.FaseId == FasesCct.IndiceFaseFechada);
                negVigentes = resultFases.Count(a => a.FaseId != FasesCct.IndiceFaseFechada);
            }

            var negAbertoEstadoString = new StringBuilder(@"select DISTINCT l.uf, COUNT(DISTINCT ct.id) as quantidade from acompanhamento_cct_sindicato_laboral_tb acslt
                                                INNER JOIN acompanhamento_cct_tb ct on acslt.acompanhamento_cct_id  = ct.id
                                                LEFT JOIN base_territorialsindemp bt on acslt.sindicato_id = bt.sind_empregados_id_sinde1
                                                LEFT JOIN localizacao l on bt.localizacao_id_localizacao1 = l.id_localizacao
                                                where acslt.acompanhamento_cct_id = ct.id
                                                and ct.fase_id not in (@faseConcluida, @faseArquivada)
                                                and EXISTS(
                                                    select 1 FROM cnae_emp as ce 
                                                    inner join classe_cnae as cc ON cc.id_cnae = ce.classe_cnae_idclasse_cnae
                                                    and JSON_CONTAINS(cast(ct.cnaes_ids as char), ct.cnaes_ids)
                                                )");

            parametrosMandatos.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parametrosMandatos.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parametrosMandatos.Add(new MySqlParameter("@faseConcluida", FasesCct.IndiceFaseConcluida));
            parametrosMandatos.Add(new MySqlParameter("@faseArquivada", FasesCct.IndiceFaseArquivada));

            int parametrosMandatosCount = 0;

            if (sindeIds != null && sindeIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(negAbertoEstadoString, sindeIds, "acslt.sindicato_id", parametrosMandatos, ref parametrosMandatosCount);
            }

            negAbertoEstadoString.Append(" group by l.uf");

            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();

            foreach (MySqlParameter parameter in parametrosMandatos)
            {
                if (parameter != null && parameter.Value != null)
                {
                    parameterDictionary[parameter.ParameterName] = parameter.Value;
                }
            }

            var resultNegAbertoEstado = (await _context.SelectFromRawSqlAsync<NegAbertoEstadoViewModel>(negAbertoEstadoString.ToString(), parameterDictionary)).ToList()
                                    ?? new List<NegAbertoEstadoViewModel>();

            var queryComentariosPorEstado = _context.ComentarioSindicatoVw
                                     .FromSql($@"select vw.* from comentario_sindicato_vw vw
	                                            inner join usuario_adm uat on uat.email_usuario = {_userInfoService.GetEmail()}
	                                            where case when uat.nivel = 'Ineditta' then true 
			                                            when uat.nivel = 'Grupo Econômico' then (
                                                            case when uat.id_user != vw.usuario_publicacao_id then (
                                                                vw.visivel = 1 AND uat.id_grupoecon = vw.usuario_publicacao_grupo_economico_id
                                                            )
                                                            else true
                                                            end
                                                        )
			                                            else (
                                                            case when uat.id_user != vw.usuario_publicacao_id then (
                                                                vw.visivel = 1 AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(vw.estabelecimento_id))
                                                            )
                                                            else true
                                                            end
                                                        )
			                                          end
                                                      and vw.notificacao_comentario is not null
                                                ");

            if (sindeIds != null && sindeIds.Any())
            {
                var sindicatosLaborais = sindeIds;
                var sindicatosPatronais = sindpIds;
                queryComentariosPorEstado = queryComentariosPorEstado.Where(csv => 
                    (csv.SindicatoId != null && csv.SindicatoTipo == "laboral" && sindicatosLaborais.Contains(csv.SindicatoId.Value)) ||
                    (csv.SindicatoId != null && csv.SindicatoTipo == "patronal" && sindicatosPatronais.Contains(csv.SindicatoId.Value))
                );
            }

            queryComentariosPorEstado.GroupBy(csv => csv.SindicatoUf);

            var resultComentariosPorEstado = await queryComentariosPorEstado
                                 .Select(com => com.SindicatoUf)
                                 .ToListAsync();

            var totalMandatos = resultEmps.Concat(resultPatrs).ToArray();

            var parametrosQtdCentraisSindicais = new List<MySqlParameter>();

            int parametrosQtdCentraisSindicaisCount = 0;

            var qtdCentraisSindicaisString = new StringBuilder($@"select cst.id_centralsindical, max(cst.nome_centralsindical) central, count(1) qtd
                                    from central_sindical cst
                                    inner join sind_emp sindempt on cst.id_centralsindical = sindempt.central_sindical_id_centralsindical");

            if (sindeIds != null && sindeIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(qtdCentraisSindicaisString, sindeIds, "sindempt.id_sinde", parametrosQtdCentraisSindicais, ref parametrosQtdCentraisSindicaisCount);
            }

            qtdCentraisSindicaisString.Append(" group by 1");

            Dictionary<string, object> parametrosQtdCentraisSindicaisDictionary = new Dictionary<string, object>();

            foreach (MySqlParameter parameter in parametrosQtdCentraisSindicais)
            {
                if (parameter != null && parameter.Value != null)
                {
                    parametrosQtdCentraisSindicaisDictionary[parameter.ParameterName] = parameter.Value;
                }
            }

            var resultQtdCentraisSindicais = (await _context.SelectFromRawSqlAsync<QuantidadeCentraisSindicaisViewModel>(qtdCentraisSindicaisString.ToString(), parametrosQtdCentraisSindicaisDictionary)).ToList()
                                    ?? new List<QuantidadeCentraisSindicaisViewModel>();

            var mandatosSindicais = new MandatosSindicaisViewModel
            {
                MandatosVencidos = totalMandatos.Count(x => x.ToDateTime(TimeOnly.MaxValue) < DateTime.Now),
                MandatosVigentes = totalMandatos.Count(x => x.ToDateTime(TimeOnly.MaxValue) >= DateTime.Now),
                TerminoMandatosEmps = resultEmps,
                TerminoMandatosPatrs = resultPatrs,
                NegVencidas = negVencidas,
                NegVigentes = negVigentes,
                NegAbertoPorEstado = resultNegAbertoEstado,
                EstadosComentarios = resultComentariosPorEstado,
                QtdCentraisSindicais = resultQtdCentraisSindicais
            };

            /// MANDATOS FINISH


            var result = new SindicatosViewModel
            {
                SindicatosLaborais = resultQuerySinde,
                SindicatosPatronais = resultQuerySindp,
                MandatosSindicais = mandatosSindicais,
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("organizacao-sindical-laboral")]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<OrganizacaoSindicalLaboralViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OrganizacaoSindicalLaboralViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterOrganizacaoSindicalLaboralAsync([FromQuery] OrganizacaoSindicalLaboralRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 from cliente_unidades cut
	                                                            inner join base_territorialsindemp bemp on bemp.sind_empregados_id_sinde1 = sinde.id_sinde
	                                                            left join base_territorialsindpatro bpatr on bpatr.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1
                                                                and bpatr.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join localizacao loc on loc.id_localizacao = bemp.localizacao_id_localizacao1
	                                                            inner join classe_cnae cc on cc.id_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
	                                                            inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join usuario_adm uat on uat.email_usuario = @email
	                                                            where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bemp.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = 'Ineditta' then true 
                                                                        when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else json_contains(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));

            if (request.GrupoEconomicoId > 0)
            {
                sindEmpregadosString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindLaboraisIds, "sinde.id_sinde", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindPatronaisIds, "bpatr.sind_patronal_id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindEmpregadosString.Append(')');

            var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(set => new SindicatoLaboralViewModel
                    {
                        Id = set.Id,
                        Cnpj = set.Cnpj.Value,
                        RazaoSocial = set.RazaoSocial ?? string.Empty,
                        Municipio = set.Municipio ?? string.Empty,
                        Uf = set.Uf ?? string.Empty,
                        Sigla = set.Sigla
                    });

            var resultQuerySinde = await querySinde.ToListAsync();

            var sindeIds = resultQuerySinde.Select(x => x.Id);

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from sinde in _context.SindEmps
                                    join cs in _context.CentralSindicals on sinde.CentralSindicalId equals cs.IdCentralsindical into _cs
                                    from cs in _cs.DefaultIfEmpty()
                                    join asc in _context.Associacoes on sinde.ConfederacaoId equals asc.IdAssociacao into _asc
                                    from asc in _asc.DefaultIfEmpty()
                                    join asf in _context.Associacoes on sinde.FederacaoId equals asf.IdAssociacao into _asf
                                    from asf in _asf.DefaultIfEmpty()
                                    where sindeIds != null && sindeIds.Contains(sinde.Id)
                                    select new OrganizacaoSindicalLaboralViewModel
                                    {
                                        Id = sinde.Id,
                                        Municipio = sinde.Municipio,
                                        Cnpj = sinde.Cnpj.Value,
                                        Sigla = sinde.Sigla,
                                        NomeCentralSindical = cs.NomeCentralsindical,
                                        NomeConfederacao = asc.Nome,
                                        NomeFederacao = asf.Nome,
                                    }
                                ).ToDataTableResponseAsync(request);

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = from sinde in _context.SindEmps
                            join cs in _context.CentralSindicals on sinde.CentralSindicalId equals cs.IdCentralsindical into _cs
                            from cs in _cs.DefaultIfEmpty()
                            join asc in _context.Associacoes on sinde.ConfederacaoId equals asc.IdAssociacao into _asc
                            from asc in _asc.DefaultIfEmpty()
                            join asf in _context.Associacoes on sinde.FederacaoId equals asf.IdAssociacao into _asf
                            from asf in _asf.DefaultIfEmpty()
                            where sindeIds != null && sindeIds.Contains(sinde.Id)
                            select new OrganizacaoSindicalLaboralViewModel
                            {
                                Id = sinde.Id,
                                Municipio = sinde.Municipio,
                                Cnpj = sinde.Cnpj.Value,
                                Sigla = sinde.Sigla,
                                NomeCentralSindical = cs.NomeCentralsindical,
                                NomeConfederacao = asc.Nome,
                                NomeFederacao = asf.Nome,
                            };

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("organizacao-sindical-patronal")]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<OrganizacaoSindicalPatronalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OrganizacaoSindicalPatronalViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterOrganizacaoSindicalPatronalAsync([FromQuery] OrganizacaoSindicalPatronalRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                            from sind_patr sindp
                                                            where exists(select 1 from cliente_unidades cut
                                                                inner join base_territorialsindpatro bpatr on bpatr.sind_patronal_id_sindp = sindp.id_sindp
                                                                inner join localizacao loc on loc.id_localizacao = bpatr.localizacao_id_localizacao1
                                                                inner join classe_cnae cc on cc.id_cnae = bpatr.classe_cnae_idclasse_cnae
                                                                inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
                                                                inner join cnae_emp as cet on (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join base_territorialsindemp AS bemp 
                                                                    on (bemp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                    AND bemp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                inner join usuario_adm uat on uat.email_usuario = @email
                                                                where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bpatr.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                sindPatronaisString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindLaboraisIds, "bemp.sind_empregados_id_sinde1", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindPatronaisIds, "sindp.id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindPatronaisString.Append(')');

            var querySindp = _context.SindPatrs
                .FromSqlRaw(sindPatronaisString.ToString(), parameters.ToArray())
                .AsNoTracking()
                .Select(spt => new SindicatoPatronalViewModel
                {
                    Id = spt.Id,
                    Sigla = spt.Sigla,
                    Cnpj = spt.Cnpj.Value ?? string.Empty,
                    RazaoSocial = spt.RazaoSocial ?? string.Empty,
                    Municipio = spt.Municipio ?? string.Empty,
                    Uf = spt.Uf ?? string.Empty,
                });

            var resultQuerySindp = await querySindp.ToListAsync();

            var sindpIds = resultQuerySindp.Select(x => x.Id);

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var usuarioEmail = _userInfoService.GetEmail();
                if (string.IsNullOrEmpty(usuarioEmail)) return NoContent();

                var parametersQuery = new List<MySqlParameter>();
                var parametersQueryCount = 0;

                var queryBuilder = new StringBuilder(@"
                    SELECT  
    	                sp.id_sindp id, 
    	                sp.municipio_sp municipio,
    	                sp.sigla_sp sigla,
    	                sp.cnpj_sp cnpj,
    	                a.nome nome_confederacao,
    	                a2.nome nome_federacao,
    	                associado.valor associado
                    FROM sind_patr sp 
                    LEFT JOIN associacao a ON sp.confederacao_id_associacao = a.id_associacao 
                    LEFT JOIN associacao a2 ON sp.federacao_id_associacao = a.id_associacao 
                    LEFT JOIN LATERAL (
	                    SELECT
		                    (CASE WHEN EXISTS (
			                    SELECT 1 FROM cliente_unidade_sindicato_patronal_tb cusp
			                    JOIN cliente_unidades cut ON cut.id_unidade = cusp.cliente_unidade_id
			                    JOIN usuario_adm uat ON uat.email_usuario = @usuarioEmail
			                    WHERE CASE WHEN uat.nivel = 'Ineditta' THEN TRUE 
	               	                       WHEN uat.nivel = 'Grupo Econômico' THEN uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
	                                       ELSE JSON_CONTAINS(uat.ids_fmge, cast(cut.id_unidade as char))
	                                  END
	                                  AND sp.id_sindp = cusp.sindicato_patronal_id
		                    ) THEN 'sim' ELSE 'não'
		                    END) valor  
                    ) associado ON TRUE
                    WHERE TRUE
                ");

                parametersQuery.Add(new MySqlParameter("@usuarioEmail", usuarioEmail));

                if (!sindpIds.Any())
                {
                    queryBuilder.Append(" AND FALSE");
                }
                else {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, sindpIds, "sp.id_sindp", parametersQuery, ref parametersQueryCount);
                }

                var result = await _context.OrganizacaoPatronalVws
                                .FromSqlRaw(queryBuilder.ToString(), parametersQuery.ToArray())
                                .Select(r => new OrganizacaoSindicalPatronalViewModel
                                {
                                    Id = r.SindicatoPatronalId,
                                    Cnpj = r.Cnpj,
                                    Sigla = r.Sigla,
                                    Associado = r.Associado,
                                    Municipio = r.Municipio,
                                    NomeConfederacao = r.NomeConfederacao,
                                    NomeFederacao = r.NomeFederacao
                                })
                                .ToDataTableResponseAsync(request);

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = from sindp in _context.SindPatrs
                            join asc in _context.Associacoes on sindp.ConfederacaoId equals asc.IdAssociacao into _asc
                            from asc in _asc.DefaultIfEmpty()
                            join asf in _context.Associacoes on sindp.FederacaoId equals asf.IdAssociacao into _asf
                            from asf in _asf.DefaultIfEmpty()
                            where sindpIds != null && sindpIds.Contains(sindp.Id)
                            select new OrganizacaoSindicalPatronalViewModel
                            {
                                Id = sindp.Id,
                                Municipio = sindp.Municipio,
                                Cnpj = sindp.Cnpj.Value,
                                Sigla = sindp.Sigla,
                                NomeConfederacao = asc.Nome,
                                NomeFederacao = asf.Nome,
                            };

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("dirigentes-patronais")]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<OrganizacaoSindicalPatronalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OrganizacaoSindicalPatronalViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterDirigentesPatronaisAsync([FromQuery] DirigentesPatronaisRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                            from sind_patr sindp
                                                            where exists(select 1 from cliente_unidades cut
                                                                inner join base_territorialsindpatro bpatr on bpatr.sind_patronal_id_sindp = sindp.id_sindp
                                                                inner join localizacao loc on loc.id_localizacao = bpatr.localizacao_id_localizacao1
                                                                inner join classe_cnae cc on cc.id_cnae = bpatr.classe_cnae_idclasse_cnae
                                                                inner join cnae_emp as cet on (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
                                                                inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join base_territorialsindemp AS bemp 
                                                                    on (bemp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                    AND bemp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                inner join usuario_adm uat on uat.email_usuario = @email
                                                                where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bpatr.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                sindPatronaisString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindLaboraisIds, "bemp.sind_empregados_id_sinde1", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindPatronaisIds, "sindp.id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindPatronaisString.Append(')');

            var querySindp = _context.SindPatrs
                .FromSqlRaw(sindPatronaisString.ToString(), parameters.ToArray())
                .AsNoTracking()
                .Select(spt => new SindicatoPatronalViewModel
                {
                    Id = spt.Id,
                    Sigla = spt.Sigla,
                    Cnpj = spt.Cnpj.Value ?? string.Empty,
                    RazaoSocial = spt.RazaoSocial ?? string.Empty,
                    Municipio = spt.Municipio ?? string.Empty,
                    Uf = spt.Uf ?? string.Empty,
                });

            var resultQuerySindp = await querySindp.ToListAsync();

            var sindpIds = resultQuerySindp.Select(x => x.Id);

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                var parametersDirigentes = new List<MySqlParameter>();
                var parametersDirigentesCount = 0;

                var queryDirigentes = new StringBuilder(@"select vw.* from dirigente_patronal_vw vw where true");

                if (!sindpIds.Any())
                {
                    queryDirigentes.Append(" AND FALSE");
                }
                else
                {
                    QueryBuilder.AppendListToQueryBuilder(queryDirigentes, sindpIds, "vw.sindp_id", parametersDirigentes, ref parametersDirigentesCount);
                }
                

                var result = await _context.DirigentesPatronaisVw.FromSqlRaw(queryDirigentes.ToString(), parametersDirigentes.ToArray())
                                    .AsNoTracking()
                                    .Select(vw => new DirigenteSindicalViewModel
                                    {
                                        Id = vw.Id,
                                        Nome = vw.Nome,
                                        Sigla = vw.Sigla,
                                        Situacao = request.GrupoEconomicoId == vw.Grupo ? vw.Situacao : string.Empty,
                                        Cargo = vw.Cargo,
                                        RazaoSocial = vw.RazaoSocial,
                                        InicioMandato = vw.InicioMandato.HasValue ? vw.InicioMandato.Value : null,
                                        TerminoMandato = vw.TerminoMandato.HasValue ? vw.TerminoMandato.Value : null,
                                        NomeUnidade = request.GrupoEconomicoId == vw.Grupo ? vw.NomeUnidade : string.Empty,
                                        UnidadeId = vw.UnidadeId > 0 ? vw.UnidadeId : null,
                                        SindId = vw.SindpId
                                    }).ToDataTableResponseAsync(request);


                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = from dirpatr in _context.SindDirpatros
                            join cut in _context.ClienteUnidades on dirpatr.EstabelecimentoId equals cut.Id into _cut
                            from cut in _cut.DefaultIfEmpty()
                            join cm in _context.ClienteMatrizes on cut.EmpresaId equals cm.Id into _cm
                            from cm in _cm.DefaultIfEmpty()
                            join sindp in _context.SindPatrs on dirpatr.SindicatoPatronalId equals sindp.Id into _sindp
                            from sindp in _sindp.DefaultIfEmpty()
                            where sindpIds == null || sindpIds.Contains(dirpatr.SindicatoPatronalId)
                            orderby dirpatr.Nome ascending
                            select new DirigenteSindicalViewModel
                            {
                                Id = (int)dirpatr.Id,
                                Nome = dirpatr.Nome,
                                Sigla = sindp.Sigla,
                                Situacao = dirpatr.Situacao,
                                Cargo = dirpatr.Funcao,
                                RazaoSocial = EF.Property<string>(cm, "RazaoSocial"),
                                InicioMandato = dirpatr.DataInicioMandato,
                                TerminoMandato = dirpatr.DataFimMandato,
                                NomeUnidade = cut.Nome,
                                UnidadeId = cut.Id > 0 ? cut.Id : null,
                                SindId = sindp.Id
                            };

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("dirigentes-laborais")]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<OrganizacaoSindicalPatronalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OrganizacaoSindicalPatronalViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterDirigentesLaboraisAsync([FromQuery] DirigentesLaboraisRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 from cliente_unidades cut
	                                                            inner join base_territorialsindemp bemp on bemp.sind_empregados_id_sinde1 = sinde.id_sinde
	                                                            left join base_territorialsindpatro bpatr on bpatr.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1
                                                                and bpatr.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join localizacao loc on loc.id_localizacao = bemp.localizacao_id_localizacao1
	                                                            inner join classe_cnae cc on cc.id_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
	                                                            inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join usuario_adm uat on uat.email_usuario = @email
	                                                            where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bemp.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else json_contains(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                sindEmpregadosString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindLaboraisIds, "sinde.id_sinde", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindPatronaisIds, "bpatr.sind_patronal_id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindEmpregadosString.Append(')');

            var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(set => new SindicatoLaboralViewModel
                    {
                        Id = set.Id,
                        Cnpj = set.Cnpj.Value,
                        RazaoSocial = set.RazaoSocial ?? string.Empty,
                        Municipio = set.Municipio ?? string.Empty,
                        Uf = set.Uf ?? string.Empty,
                        Sigla = set.Sigla
                    });

            var resultQuerySinde = await querySinde.ToListAsync();

            var sindeIds = resultQuerySinde.Select(x => x.Id);

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parametersDirigentes = new List<MySqlParameter>();
                var parametersDirigentesCount = 0;

                var queryDirigentes = new StringBuilder(@"select vw.* from dirigente_laboral_vw vw where true");

                if (!sindeIds.Any())
                {
                    queryDirigentes.Append(" AND FALSE");
                }
                else
                {
                    QueryBuilder.AppendListToQueryBuilder(queryDirigentes, sindeIds, "vw.sinde_id", parametersDirigentes, ref parametersDirigentesCount);
                }

                var result = await _context.DirigentesLaboraisVw.FromSqlRaw(queryDirigentes.ToString(), parametersDirigentes.ToArray())
                                    .AsNoTracking()
                                    .Select(vw => new DirigenteSindicalViewModel
                                    {
                                        Id = vw.Id,
                                        Nome = vw.Nome,
                                        Sigla = vw.Sigla,
                                        Situacao = request.GrupoEconomicoId == vw.Grupo ? vw.Situacao : string.Empty,
                                        Cargo = vw.Cargo,
                                        RazaoSocial = vw.RazaoSocial,
                                        InicioMandato = vw.InicioMandato.HasValue ? vw.InicioMandato.Value : null,
                                        TerminoMandato = vw.TerminoMandato.HasValue ? vw.TerminoMandato.Value : null,
                                        NomeUnidade = request.GrupoEconomicoId == vw.Grupo ? vw.NomeUnidade : string.Empty,
                                        UnidadeId = vw.UnidadeId > 0 ? vw.UnidadeId : null,
                                        SindId = vw.SindeId
                                    }).ToDataTableResponseAsync(request); 

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = from diremp in _context.SindDiremps
                            join cut in _context.ClienteUnidades on diremp.ClienteUnidadesIdUnidade equals cut.Id into _cut
                            from cut in _cut.DefaultIfEmpty()
                            join cm in _context.ClienteMatrizes on cut.EmpresaId equals cm.Id into _cm
                            from cm in _cm.DefaultIfEmpty()
                            join sinde in _context.SindEmps on diremp.SindEmpIdSinde equals sinde.Id into _sinde
                            from sinde in _sinde.DefaultIfEmpty()
                            where sindeIds == null || sindeIds.Contains(diremp.SindEmpIdSinde)
                            orderby diremp.DirigenteE ascending
                            select new DirigenteSindicalViewModel
                            {
                                Id = diremp.IdDiretoriae,
                                Nome = diremp.DirigenteE,
                                Sigla = sinde.Sigla,
                                Situacao = diremp.SituacaoE,
                                Cargo = diremp.FuncaoE,
                                RazaoSocial = EF.Property<string>(cm, "RazaoSocial"),
                                InicioMandato = diremp.InicioMandatoe,
                                TerminoMandato = diremp.TerminoMandatoe,
                                NomeUnidade = cut.Nome,
                                UnidadeId = cut.Id > 0 ? cut.Id : null,
                                SindId = sinde.Id
                            };

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }
#pragma warning restore CS8629 // Nullable value type may be null.

            return NotAcceptable();
        }

        [HttpGet]
        [Route("infos-sindicais")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(InfoSindicalViewModel), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterInfosSindicaisAsync([FromQuery] InfoDirigentesRequest request)
        {
            if (request.TipoSind == "laboral")
            {
                var result = await (from sinde in _context.SindEmps
                                    join cst in _context.CentralSindicals on sinde.CentralSindicalId equals cst.IdCentralsindical into _cst
                                    from cst in _cst.DefaultIfEmpty()
                                    join cft in _context.Associacoes on sinde.ConfederacaoId equals cft.IdAssociacao into _cft
                                    from cft in _cft.DefaultIfEmpty()
                                    join ft in _context.Associacoes on sinde.FederacaoId equals ft.IdAssociacao into _ft
                                    from ft in _ft.DefaultIfEmpty()
                                    where sinde.Id == request.SindId
                                    select new InfoSindicalViewModel
                                    {
                                        Sigla = sinde.Sigla,
                                        Cnpj = sinde.Cnpj.Value,
                                        RazaoSocial = sinde.RazaoSocial,
                                        Denominacao = sinde.Denominacao,
                                        CodigoSindical = sinde.CodigoSindical.Valor,
                                        Uf = sinde.Uf,
                                        Municipio = sinde.Municipio,
                                        Logradouro = sinde.Logradouro,
                                        Telefone1 = sinde.Telefone1.Valor,
                                        Telefone2 = sinde.Telefone2 == null ? default : sinde.Telefone2!.Valor,
                                        Telefone3 = sinde.Telefone3 == null ? default : sinde.Telefone3!.Valor,
                                        Site = sinde.Site,
                                        Email1 = sinde.Email1 == null ? default : sinde.Email1!.Valor,
                                        Email2 = sinde.Email2 == null ? default : sinde.Email2!.Valor,
                                        Email3 = sinde.Email3 == null ? default : sinde.Email3!.Valor,
                                        Facebook = sinde.Facebook,
                                        Twitter = sinde.Twitter,
                                        Instagram = sinde.Instagram,
                                        Ramal = sinde.Ramal == null ? default : sinde.Ramal!.Valor,
                                        ContatoEnquadramento = sinde.Enquadramento,
                                        ContatoContribuicao = sinde.Contribuicao,
                                        ContatoNegociador = sinde.Negociador,
                                        Federacao = new AssociacaoSindicatoViewModel
                                        {
                                            Nome = ft.Nome,
                                            Cnpj = ft.Cnpj,
                                        },
                                        Confederacao = new AssociacaoSindicatoViewModel
                                        {
                                            Nome = cft.Nome,
                                            Cnpj = cft.Cnpj,
                                        },
                                        CentralSindical = new AssociacaoSindicatoViewModel
                                        {
                                            Nome = cst.NomeCentralsindical,
                                            Cnpj = cst.Cnpj
                                        }
                                    })
                                    .AsNoTracking()
                                    .SingleAsync();

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (request.TipoSind == "patronal")
            {
                var result = await (from sindp in _context.SindPatrs
                                    join cft in _context.Associacoes on sindp.ConfederacaoId equals cft.IdAssociacao into _cft
                                    from cft in _cft.DefaultIfEmpty()
                                    join ft in _context.Associacoes on sindp.FederacaoId equals ft.IdAssociacao into _ft
                                    from ft in _ft.DefaultIfEmpty()
                                    where sindp.Id == request.SindId
                                    select new InfoSindicalViewModel
                                    {
                                        Sigla = sindp.Sigla,
                                        Cnpj = sindp.Cnpj.Value,
                                        RazaoSocial = sindp.RazaoSocial,
                                        Denominacao = sindp.Denominacao,
                                        CodigoSindical = sindp.CodigoSindical.Valor,
                                        Uf = sindp.Uf,
                                        Municipio = sindp.Municipio,
                                        Logradouro = sindp.Logradouro,
                                        Telefone1 = sindp.Telefone1.Valor,
                                        Telefone2 = sindp.Telefone2 == null ? default : sindp.Telefone2!.Valor,
                                        Telefone3 = sindp.Telefone3 == null ? default : sindp.Telefone3!.Valor,
                                        Site = sindp.Site,
                                        Email1 = sindp.Email1 == null ? default : sindp.Email1!.Valor,
                                        Email2 = sindp.Email2 == null ? default : sindp.Email2!.Valor,
                                        Email3 = sindp.Email3 == null ? default : sindp.Email3!.Valor,
                                        Facebook = sindp.Facebook,
                                        Twitter = sindp.Twitter,
                                        Instagram = sindp.Instagram,
                                        Ramal = sindp.Ramal == null ? default : sindp.Ramal!.Valor,
                                        ContatoEnquadramento = sindp.Enquadramento,
                                        ContatoContribuicao = sindp.Contribuicao,
                                        ContatoNegociador = sindp.Negociador,
                                        Federacao = new AssociacaoSindicatoViewModel
                                        {
                                            Nome = ft.Nome,
                                            Cnpj = ft.Cnpj,
                                        },
                                        Confederacao = new AssociacaoSindicatoViewModel
                                        {
                                            Nome = cft.Nome,
                                            Cnpj = cft.Cnpj,
                                        }
                                    })
                                .AsNoTracking()
                                .SingleAsync();

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("info-dirigentes")]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<InfoDirigenteViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterInfoDirigentesAsync([FromQuery] InfoDirigentesRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                if (request.TipoSind == "laboral")
                {

                    return Ok(await _context.SindDiremps
                                                .AsNoTracking()
                                                .Where(dir => dir.SindEmpIdSinde == request.SindId)
                                                .Select(x => new InfoDirigenteViewModel
                                                {
                                                    Nome = x.DirigenteE,
                                                    InicioMandato = x.InicioMandatoe,
                                                    FimMandato = x.TerminoMandatoe,
                                                    Funcao = x.FuncaoE
                                                })
                                                .ToDataTableResponseAsync(request));
                }

                if (request.TipoSind == "patronal")
                {
                    var result = await _context.SindDirpatros
                            .AsNoTracking()
                            .Where(dir => dir.SindicatoPatronalId == request.SindId)
                            .Select(x => new InfoDirigenteViewModel
                            {
                                Nome = x.Nome,
                                InicioMandato = x.DataInicioMandato,
                                FimMandato = x.DataFimMandato,
                                Funcao = x.Funcao
                            }).ToDataTableResponseAsync(request);

                    return Ok(result);
                }
            }

            return NoContent();
        }
        [HttpGet]
        [Route("comentario-uf")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<SindicatoPorUFDataTableViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterComentariosPorUFAsync([FromQuery] SindCommentRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 from cliente_unidades cut
	                                                            inner join base_territorialsindemp bemp on bemp.sind_empregados_id_sinde1 = sinde.id_sinde
	                                                            left join base_territorialsindpatro bpatr on bpatr.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1
                                                                and bpatr.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join localizacao loc on loc.id_localizacao = bemp.localizacao_id_localizacao1
	                                                            inner join classe_cnae cc on cc.id_cnae = bemp.classe_cnae_idclasse_cnae
	                                                            inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
	                                                            inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join usuario_adm uat on uat.email_usuario = @email
	                                                            where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bemp.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                            from sind_patr sindp
                                                            where exists(select 1 from cliente_unidades cut
                                                                inner join base_territorialsindpatro bpatr on bpatr.sind_patronal_id_sindp = sindp.id_sindp
                                                                inner join localizacao loc on loc.id_localizacao = bpatr.localizacao_id_localizacao1
                                                                inner join classe_cnae cc on cc.id_cnae = bpatr.classe_cnae_idclasse_cnae
                                                                inner join cnae_emp as cet on (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
                                                                inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                                                inner join base_territorialsindemp AS bemp 
                                                                    on (bemp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                    AND bemp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                inner join usuario_adm uat on uat.email_usuario = @email 
                                                                where cut.localizacao_id_localizacao = loc.id_localizacao 
                                                                and JSON_CONTAINS(cut.cnae_unidade, concat('{{""id"":', bpatr.classe_cnae_idclasse_cnae,'}}'))
                                                                and case WHEN uat.nivel = @nivel then true 
                                                                        when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));

            if (request.GrupoEconomicoId > 0)
            {
                sindEmpregadosString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                sindPatronaisString.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.LocalizacoesIds, "loc.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Regioes, "loc.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
                QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.CnaesIds, "cut.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindLaboraisIds, "sinde.id_sinde", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindLaboraisIds, "bemp.sind_empregados_id_sinde1", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.SindPatronaisIds, "bpatr.sind_patronal_id_sindp", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.SindPatronaisIds, "sindp.id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
                QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.DataBase, "bemp.dataneg", parameters, ref parametersCount);
            }

            sindEmpregadosString.Append(')');
            sindPatronaisString.Append(')');

            var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(set => new SindicatoLaboralViewModel
                    {
                        Id = set.Id,
                        Cnpj = set.Cnpj.Value,
                        RazaoSocial = set.RazaoSocial ?? string.Empty,
                        Municipio = set.Municipio ?? string.Empty,
                        Uf = set.Uf ?? string.Empty,
                        Sigla = set.Sigla
                    });

            var querySindp = _context.SindPatrs
                .FromSqlRaw(sindPatronaisString.ToString(), parameters.ToArray())
                .AsNoTracking()
                .Select(spt => new SindicatoPatronalViewModel
                {
                    Id = spt.Id,
                    Sigla = spt.Sigla,
                    Cnpj = spt.Cnpj.Value ?? string.Empty,
                    RazaoSocial = spt.RazaoSocial ?? string.Empty,
                    Municipio = spt.Municipio ?? string.Empty,
                    Uf = spt.Uf ?? string.Empty,
                });

            var resultQuerySinde = await querySinde.ToListAsync();
            var resultQuerySindp = await querySindp.ToListAsync();

            var sindeIds = resultQuerySinde.Select(x => x.Id);
            var sindpIds = resultQuerySindp.Select(x => x.Id);

#pragma warning disable CA1305 // Specify IFormatProvider
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parametros = new List<MySqlParameter>
                {
                    new MySqlParameter("@userEmail", _userInfoService.GetEmail()),
                    new MySqlParameter("@uf", request.Uf)
                };

                var parametrosSindeIds = new List<string>();
                var parametrosSindpIds = new List<string>();

                foreach (int sindeId in sindeIds)
                {
                    parametrosSindeIds.Add("@sindeIds" + sindeId);
                    parametros.Add(new MySqlParameter("@sindeIds" + sindeId, sindeId));
                }

                foreach (int sindpId in sindpIds)
                {
                    parametrosSindpIds.Add("@sindeIds" + sindpId);
                    parametros.Add(new MySqlParameter("@sindeIds" + sindpId, sindpId));
                }

                var query = _context.ComentarioSindicatoVw
                                     .FromSqlRaw($@"WITH SindicatoComentarios AS (
	                                                select vw.* from comentario_sindicato_vw vw
	                                                inner join usuario_adm uat on uat.email_usuario = @userEmail
	                                                where vw.sindicato_localizacao_uf = @uf 
	                                                    and case when uat.nivel = 'Ineditta' then true 
	                                                             when uat.nivel = 'Grupo Econômico' then (
                                                                        case when uat.id_user != vw.usuario_publicacao_id then (
                                                                            vw.visivel = 1 AND uat.id_grupoecon = vw.usuario_publicacao_grupo_economico_id
                                                                        )
                                                                        else 
                                                                            true
                                                                        end
                                                                    )
			                                                        else (
                                                                        case when uat.id_user != vw.usuario_publicacao_id then (
                                                                            vw.visivel = 1 AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(vw.estabelecimento_id))
                                                                        )
                                                                        else true
                                                                        end
                                                                    )
	                                                            end
	                                                    or (vw.notificacao_id is null and vw.sindicato_localizacao_uf = @uf)
                                                ),

                                                SindicatoComentarioIneditta AS (
	                                                select vw.* from comentario_sindicato_vw vw
	                                                inner join usuario_adm uat on uat.email_usuario = @userEmail
	                                                where vw.sindicato_localizacao_uf = @uf 
	                                                    or (vw.notificacao_id is null and vw.sindicato_localizacao_uf = @uf)
                                                ),

                                                ColunasNulas AS (
	                                                SELECT
	                                                    null notificacao_tipo_comentario,
                                                        null notificacao_comentario,
                                                        null notificacao_id,
	                                                    null notificacao_etiqueta,
                                                        null notificacao_data_registro,
                                                        null notificacao_tipo_destino,
	                                                    null notificacao_tipo_destino_id,
                                                        null usuario_publicacao_id,
                                                        null usuario_publicacao,
	                                                    null usuario_publicacao_grupo_economico_id,
                                                        null usuario_publicacao_tipo,
	                                                    null sindicato_localizacao_uf,
                                                        null estabelecimento_id,
                                                        null empresa_id,
                                                        null grupo_economico_id,
                                                        null visivel
                                                )
                                                -- Seleciona os sindicatos e seus comentários
                                                SELECT i.* FROM SindicatoComentarios i
                                                WHERE ((`i`.`sindicato_id` IS NOT NULL AND (`i`.`tipo` = 'laboral')) AND CASE WHEN {parametrosSindeIds.Count} > 0 then `i`.`sindicato_id` IN ({(parametrosSindeIds.Any() ? string.Join(",", parametrosSindeIds) : "0")}) else true end) 
	                                                  OR ((`i`.`sindicato_id` IS NOT NULL AND (`i`.`tipo` = 'patronal')) AND CASE WHEN {parametrosSindpIds.Count} > 0 then `i`.`sindicato_id` IN ({(parametrosSindpIds.Any() ? string.Join(",", parametrosSindpIds) : "0")}) else true end)

                                                UNION ALL

                                                -- Seleciona os sindicatos patronais que podem não ter aparecido na query acima por ter comentário de um grupo diferente
                                                SELECT j.sindicato_id, j.sindicato_sigla, j.sindicato_razao_social, j.tipo, cn.*
                                                FROM SindicatoComentarioIneditta j
                                                JOIN ColunasNulas cn on true 
                                                WHERE ((`j`.`sindicato_id` IS NOT NULL AND (`j`.`tipo` = 'patronal'))  AND CASE WHEN {parametrosSindpIds.Count} > 0 then `j`.`sindicato_id` IN ({(parametrosSindpIds.Any() ? string.Join(",", parametrosSindpIds) : "0")}) else true end)
	                                                  AND j.sindicato_id NOT IN (
	  	                                                SELECT i.sindicato_id FROM SindicatoComentarios i
	  	                                                WHERE ((`i`.`sindicato_id` IS NOT NULL AND (`i`.`tipo` = 'patronal')) AND CASE WHEN {parametrosSindpIds.Count} > 0 then `i`.`sindicato_id` IN ({(parametrosSindpIds.Any() ? string.Join(",", parametrosSindpIds) : "0")}) else true end)
	                                                  )
                                                GROUP BY j.sindicato_id


                                                -- Seleciona os sindicatos laborais que podem não ter aparecido na query acima por ter comentário de um grupo diferente
                                                UNION ALL
                                                SELECT j.sindicato_id, j.sindicato_sigla, j.sindicato_razao_social, j.tipo, cn.*
                                                FROM SindicatoComentarioIneditta j
                                                JOIN ColunasNulas cn on true
                                                WHERE ((`j`.`sindicato_id` IS NOT NULL AND (`j`.`tipo` = 'laboral')) AND CASE WHEN {parametrosSindeIds.Count} > 0 then `j`.`sindicato_id` IN ({(parametrosSindeIds.Any() ? string.Join(",", parametrosSindeIds) : "0" )}) else true end ) 
	                                                  AND j.sindicato_id NOT IN (
	  	                                                SELECT i.sindicato_id FROM SindicatoComentarios i
	  	                                                WHERE ((`i`.`sindicato_id` IS NOT NULL AND (`i`.`tipo` = 'laboral')) AND CASE WHEN {parametrosSindeIds.Count} > 0 then `i`.`sindicato_id` IN ({(parametrosSindeIds.Any() ? string.Join(",", parametrosSindeIds) : "0")}) else true end) 
	                                                  )
                                                GROUP BY j.sindicato_id",
                                                parametros.ToArray()
                                                );

                var result = await query
                                 .Select(com => new SindicatoPorUFDataTableViewModel
                                 {
                                     SindId = com.SindicatoId,
                                     Sigla = com.SindicatoSigla,
                                     SindTipo = com.SindicatoTipo,
                                     SindRazaoSocial = com.SindicatoRazaoSocial,
                                     Comentario = com.Comentario,
                                     Etiqueta = com.ComentarioEtiqueta,
                                     UsuarioResponsavel = com.ComentarioResponsavelNome,
                                     CriadoEm = com.ComentarioDataRegistro,
                                 })
                                 .ToDataTableResponseAsync(request);

                return result is null ? NoContent() : Ok(result);
            }

            return NotAcceptable();
        }

        [HttpPost("relatorio")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterRelatorioSindicalAsync([FromBody] SindicatosRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var relatorio = await _sindicatoBuilder.HandleAsync(request);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            return DownloadExcelAsync("sindicatos", relatorio.Value);
        }

        [HttpPost("relatorio/laborais")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterRelatorioSindicalLaboralAsync([FromBody] SindicatosRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var relatorio = await _sindicatoLaboralBuilder.HandleAsync(request);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            return DownloadExcelAsync("sindicatos-laborais", relatorio.Value);
        }

        [HttpPost("relatorio/patronais")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterRelatorioSindicalPatronalAsync([FromBody] SindicatosRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var relatorio = await _sindicatoPatronalBuilder.HandleAsync(request);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            return DownloadExcelAsync("sindicatos-patronais", relatorio.Value);
        }
    }
}
