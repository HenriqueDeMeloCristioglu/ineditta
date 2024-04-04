using Ineditta.API.ViewModels.EventosCalendario;
using Ineditta.Application.Usuarios.Entities;
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

using MySqlConnector;

using System.Net.Mime;
using System.Text;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.Application.CalendarioSindicais.Usuarios.UseCases.Upsert;
using Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Upsert;
using QueryBuilder = Ineditta.Repository.Extensions.QueryBuilder;
using Ineditta.API.ViewModels.EventosCalendario.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.API.ViewModels.SindicatosLaborais.Requests;
using Ineditta.API.Factories.Sindicatos;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/eventos-calendario")]
    [ApiController]
    public class EventosCalendarioController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly SindicatoLaboralFactory _sindicatoLaboralFactory;
        private readonly SindicatoPatronalFactory _sindicatoPatronalFactory;
        private readonly HttpRequestItemService _httpRequestItemService;

        public EventosCalendarioController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, IUsuarioRepository usuarioRepository, HttpRequestItemService httpRequestItemService, SindicatoLaboralFactory sindicatoLaboralFactory, SindicatoPatronalFactory sindicatoPatronalFactory) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
            _usuarioRepository = usuarioRepository;
            _httpRequestItemService = httpRequestItemService;
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
        }

        [HttpGet]
        [Route("vencimentos-documentos")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioVencimentoDocumento>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterEventosCalendarioAsync([FromQuery] EventoCalendarioRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var emailUsuario = _userInfoService.GetEmail();

                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@" 
                    SELECT
                        cs.data_referencia `data`,
                        'Vencimento de Documentos' tipo_evento,
                        CASE WHEN cs.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS `origem`,
                        cs.chave_referencia chave_referencia,
                        ds.validade_inicial validade_inicial,
                        ds.validade_final validade_final,
                        ds.tipo_doc_idtipo_doc tipo_doc_id,
                        td.nome_doc tipo_doc_nome,
                        ds.cnae_doc atividades_economicas_json,
                        ds.cliente_estabelecimento cliente_estabelecimento,
                        ds.sind_laboral sind_laboral,
                        ds.sind_patronal sind_patronal,
                        ds.referencia referencia,
                        replace(replace(replace(json_extract(`ds`.`cnae_doc`, '$[*].subclasse'), '""', ''), '[', ''), ']', '') AS `atividades_economicas`
                    FROM calendario_sindical_tb cs
                    INNER JOIN doc_sind ds ON ds.id_doc = cs.chave_referencia
                    INNER JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                    WHERE EXISTS (
                        SELECT * FROM documento_estabelecimento_tb det
                        INNER JOIN cliente_unidades cut ON cut.id_unidade = det.estabelecimento_id
                        INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                        WHERE CASE WHEN uat.nivel = 'Ineditta' THEN TRUE
                                   WHEN uat.nivel = 'Grupo Econômico' THEN uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                   ELSE JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade))
                              END
                              AND det.documento_id = cs.chave_referencia
                    )
                        AND cs.tipo_evento = @tipoEvento
                ");

                parameters.Add(new MySqlParameter("@email", emailUsuario));
                parameters.Add(new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.VencimentoDocumento.Id));

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND cs.data_referencia BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND cs.data_referencia >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.NomesDocumentosIds is not null && request.NomesDocumentosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.NomesDocumentosIds, "ds.tipo_doc_idtipo_doc", parameters, ref parametersCount);
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "cs.origem", parameters, ref parametersCount);
                }

                if (request.AtividadesEconomicasIds is not null && request.AtividadesEconomicasIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.AtividadesEconomicasIds, "ds.cnae_doc", "id", parameters, ref parametersCount);
                }

                if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.GruposEconomicosIds, "ds.cliente_estabelecimento", "g", parameters, ref parametersCount);
                }

                if (request.MatrizesIds is not null && request.MatrizesIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.MatrizesIds, "ds.cliente_estabelecimento", "m", parameters, ref parametersCount);
                }

                if (request.EstabelecimentosIds is not null && request.EstabelecimentosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.EstabelecimentosIds, "ds.cliente_estabelecimento", "u", parameters, ref parametersCount);
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds, "ds.sind_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds, "ds.sind_patronal", "id", parameters, ref parametersCount);
                }

                if (request.AssuntosIds is not null && request.AssuntosIds.Any())
                {
                    QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, request.AssuntosIds, "ds.referencia", parameters, ref parametersCount);
                }

                if (request.GruposAssuntoIds is not null && request.GruposAssuntoIds.Any())
                {
                    var assuntosValidos = await _context.EstruturaClausula
                        .Where(ec => request.GruposAssuntoIds.ToList().Contains(ec.GrupoClausulaId!))
                        .Select(ec => ec.Id)
                        .ToListAsync();
                    QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, assuntosValidos, "ds.referencia", parameters, ref parametersCount);
                }                

                var query = _context.EventoCalendarioVencimentoDocumentoVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioVencimentoDocumento
                            {
                                Data = ec.Data,
                                TipoEvento = ec.TipoEvento,
                                NomeDocumento = ec.TipoDocNome,
                                Origem = ec.Origem,
                                AtividadesEconomicas = ec.AtividadesEconomicas,
                                ValidadeInicial = ec.ValidadeInicial,
                                ValidadeFinal = ec.ValidadeFinal,
                                SindicatosLaborais = ec.SindicatosLaborais,
                                SindicatosPatronais = ec.SindicatosPatronais,
                                ChaveReferenciaId = ec.ChaveReferenciaId,
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("assembleias-reunioes")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterAssembleiasReuniaoAsync([FromQuery] EventoCalendarioRequest request)
        {
            var user = _httpRequestItemService.ObterUsuario();

            if (user == null)
            {
                return Unauthorized("Você precisa estar autenticado.");
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                IEnumerable<int>? sindeIds = null;
                IEnumerable<int>? sindpIds = null;
                if ((request.SindicatosLaboraisIds is null || !request.SindicatosLaboraisIds.Any()) || (request.SindicatosPatronaisIds is null || !request.SindicatosPatronaisIds.Any()))
                {
                    var parametersSind = new List<MySqlParameter>();
                    var parametersSindCount = 0;

                    var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 
			                                                            from cliente_matriz as cmt 
                                                                        inner join usuario_adm uat on uat.email_usuario = @email
                                                                        LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
                                                                        LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                        LEFT join base_territorialsindemp AS bl 
                                                                            on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                            AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                        LEFT JOIN localizacao loc on bl.localizacao_id_localizacao1 = loc.id_localizacao
                                                                        LEFT join base_territorialsindpatro AS bp 
					                                                        on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                                                        AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
		                                                                 where 
		                                                                 case WHEN uat.nivel = @nivel then true 
                                                                         when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                         else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                                                                         and  sinde.id_sinde = bl.sind_empregados_id_sinde1");

                    var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                        from sind_patr sindp
                                                        where exists(select 1 
			                                                        from cliente_matriz as cmt 
                                                                    inner join usuario_adm uat on uat.email_usuario = @email
                                                                    LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
                                                                    LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                    LEFT join base_territorialsindpatro AS bp 
                                                                            on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                            AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
                                                                    LEFT JOIN localizacao loc on bp.localizacao_id_localizacao1 = loc.id_localizacao
                                                                    LEFT join base_territorialsindemp AS bl 
					                                                        on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                                                        AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
		                                                             where 
		                                                             case WHEN uat.nivel = @nivel then true 
                                                                          when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                     else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                                                                     and sindp.id_sindp = bp.sind_patronal_id_sindp");

                    parametersSind.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
                    parametersSind.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
                    parametersSind.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                    if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parametersSind, ref parametersSindCount);
                    }

                    if (request.MatrizesIds != null && request.MatrizesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parametersSind, ref parametersSindCount);
                    }

                    if (request.EstabelecimentosIds != null && request.EstabelecimentosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.EstabelecimentosIds, "cut.id_unidade", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.EstabelecimentosIds, "cut.id_unidade", parametersSind, ref parametersSindCount);
                    }

                    if ((request.MunicipiosIds != null && request.MunicipiosIds.Any()) || (request.Ufs != null && request.Ufs.Any()))
                    {
                        if ((request.MunicipiosIds != null && request.MunicipiosIds.Any()) && (request.Ufs != null && request.Ufs.Any()))
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MunicipiosIds, "loc.id_localizacao", request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MunicipiosIds, "loc.id_localizacao", request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                        }
                        else if (request.MunicipiosIds != null && request.MunicipiosIds.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MunicipiosIds, "loc.id_localizacao", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MunicipiosIds, "loc.id_localizacao", parametersSind, ref parametersSindCount);
                        }
                        else if (request.Ufs != null && request.Ufs.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                        }
                    }

                    if (request.AtividadesEconomicasIds != null && request.AtividadesEconomicasIds.Any())
                    {
                        QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.AtividadesEconomicasIds, "cut.cnae_unidade", "id", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.AtividadesEconomicasIds, "cut.cnae_unidade", "id", parametersSind, ref parametersSindCount);
                    }

                    sindPatronaisString.Append(')');
                    sindEmpregadosString.Append(')');

                    var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parametersSind.ToArray())
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

                    var querySindp = _context.SindPatrs
                    .FromSqlRaw(sindPatronaisString.ToString(), parametersSind.ToArray())
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

                    sindeIds = resultQuerySinde.Select(x => x.Id);
                    sindpIds = resultQuerySindp.Select(x => x.Id);
                }

                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@" 
                    SELECT * FROM calendario_sindical_assembleia_reuniao_vw vw
                    WHERE (vw.tipo_evento_id = @tipoEventoAssembleia OR vw.tipo_evento_id = @tipoEventoReuniao)
                ");

                parameters.Add(new MySqlParameter("@tipoEventoAssembleia", TipoEventoCalendarioSindical.AssembleiaPatronal.Id));
                parameters.Add(new MySqlParameter("@tipoEventoReuniao", TipoEventoCalendarioSindical.ReuniaoEntrePartes.Id));

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND vw.data_referencia BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND vw.data_referencia >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.NomesDocumentosIds is not null && request.NomesDocumentosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.NomesDocumentosIds, "vw.tipo_doc_id", parameters, ref parametersCount);
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "vw.origem", parameters, ref parametersCount);
                }

                if (request.AtividadesEconomicasIds is not null && request.AtividadesEconomicasIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.AtividadesEconomicasIds, "vw.atividades_economicas_ids", "id", parameters, ref parametersCount);
                }

                if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, request.GruposEconomicosIds, "vw.grupos_economicos_ids", parameters, ref parametersCount);
                }

                if (request.MatrizesIds is not null && request.MatrizesIds.Any())
                {
                    QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, request.MatrizesIds, "vw.matrizes_ids", parameters, ref parametersCount);
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    queryBuilder.Append(@"
                        and exists (
	                        select 1 as sindicatos from acompanhamento_cct_sindicato_laboral_tb acslt
	                        WHERE acslt.acompanhamento_cct_id = vw.id
                    ");

                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds, "acslt.sindicato_id", parameters, ref parametersCount);

                    queryBuilder.Append(')');
                }
                else if (user.Nivel != Nivel.Ineditta)
                {
                    queryBuilder.Append(@"
                        and exists (
	                        select 1 as sindicatos from acompanhamento_cct_sindicato_laboral_tb acslt
	                        WHERE acslt.acompanhamento_cct_id = vw.id
                    ");

                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, sindeIds!, "acslt.sindicato_id", parameters, ref parametersCount);

                    queryBuilder.Append(')');
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    queryBuilder.Append(@"
                        and exists (
	                        select 1 as sindicatos from acompanhamento_cct_sindicato_patronal_tb acspt
	                        WHERE acspt.acompanhamento_cct_id = vw.id
                    ");

                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds, "acspt.sindicato_id", parameters, ref parametersCount);

                    queryBuilder.Append(')');
                }
                else if (user.Nivel != Nivel.Ineditta)
                {
                    queryBuilder.Append(@"
                        and exists (
	                        select 1 as sindicatos from acompanhamento_cct_sindicato_patronal_tb acspt
	                        WHERE acspt.acompanhamento_cct_id = vw.id
                    ");

                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, sindpIds!, "acspt.sindicato_id", parameters, ref parametersCount);

                    queryBuilder.Append(')');
                }

                var query = _context.EventoCalendarioAssembleiaReuniaoVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioViewModel
                            {
                                DataHora = ec.Data,
                                TipoEvento = ec.TipoEvento,
                                NomeDocumento = ec.TipoDocNome,
                                Origem = ec.Origem,
                                SindicatosLaborais = ec.SindicatosLaborais,
                                SindicatosPatronais = ec.SindicatosPatronais,
                                AtividadesEconomicas = ec.DescricoesSubclasse,
                                ChaveReferenciaId = ec.ChaveReferenciaId,
                                DataBaseNegociacao = ec.DataBaseNegociacao,
                                Fase = ec.Fase
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Route("vencimentos-documentos")]
        public async ValueTask<IActionResult> CriarEventoVencimentoDocumento([FromBody] UpsertEventoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpGet]
        [Route("vencimentos-mandatos-patronais")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioVencimentoMandatoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<EventoCalendarioVencimentoMandatoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterEventosVencimentosMandatosPatronaisAsync([FromQuery] EventoCalendarioRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                IEnumerable<int>? sindicatosPatronaisIds = null;
                if (request.SindicatosPatronaisIds is null || !request.SindicatosPatronaisIds.Any())
                {
                    var sindicatoPatronalRequest = new SindicatoPatronalRequest
                    {
                        GruposEconomicosIds = request.GruposEconomicosIds,
                        MatrizesIds = request.MatrizesIds,
                        ClientesUnidadesIds = request.EstabelecimentosIds,
                        CnaesIds = request.AtividadesEconomicasIds,
                        LocalizacoesIds = request.MunicipiosIds,
                        Ufs = request.Ufs
                    };

                    var sindicatosPatronais = await _sindicatoPatronalFactory.CriarPorUsuario(sindicatoPatronalRequest);

                    sindicatosPatronaisIds = sindicatosPatronais is not null ? 
                        sindicatosPatronais.Select(sindicatoPatronal => sindicatoPatronal.Id) : 
                        null;
                }

                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@"
                    SELECT 
                        cs.data_referencia `data`,
                        CASE WHEN cs.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS `origem`,
                        `sp`.`id_sindp` AS `sindicato_id`,
                        `sd`.`inicio_mandatop` AS `validade_inicial`,
                        `sd`.`termino_mandatop` AS `validade_final`,
                        `sp`.`sigla_sp` AS `siglas_sindicatos_patronais`,
                        `sp`.`id_sindp` AS `sindicato_patronal_id`
                    FROM calendario_sindical_tb cs
                    INNER JOIN sind_patr sp ON `sp`.`id_sindp` = cs.chave_referencia
                    LEFT JOIN LATERAL (
        	            SELECT * FROM sind_dirpatro sdi 
        	            WHERE sdi.termino_mandatop = cs.data_referencia AND sdi.sind_patr_id_sindp = sp.id_sindp LIMIT 1
                    ) AS sd ON true
                    WHERE cs.tipo_evento = @tipoEvento
                ");

                parameters.Add(new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.VencimentoMandatoSindicalPatronal.Id));

                if (sindicatosPatronaisIds is not null && sindicatosPatronaisIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, sindicatosPatronaisIds, "sp.id_sindp", parameters, ref parametersCount);
                }
                else
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds!, "sp.id_sindp", parameters, ref parametersCount);
                }

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND cs.data_referencia BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND cs.data_referencia >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "cs.origem", parameters, ref parametersCount);
                }

                var query = _context.EventoCalendarioVencimentoMandatoPatronalVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioVencimentoMandatoViewModel
                            {
                                SindId = ec.SindId,
                                Data = ec.Data,
                                Origem = ec.Origem,
                                ValidadeInicial = ec.ValidadeInicial,
                                ValidadeFinal = ec.ValidadeFinal,
                                SiglaSindicato = ec.SiglasSindicatosPatronais,
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("vencimentos-mandatos-laborais")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioVencimentoMandatoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<EventoCalendarioVencimentoMandatoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterEventosVencimentosMandatosLaboraisAsync([FromQuery] EventoCalendarioRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                IEnumerable<int>? sindicatosLaboraisIds = null;
                if (request.SindicatosLaboraisIds is null || !request.SindicatosLaboraisIds.Any())
                {
                    var sindicatoLaboralRequest = new SindicatoLaboralRequest
                    {
                        GruposEconomicosIds = request.GruposEconomicosIds,
                        MatrizesIds = request.MatrizesIds,
                        ClientesUnidadesIds = request.EstabelecimentosIds,
                        CnaesIds = request.AtividadesEconomicasIds,
                        LocalizacoesIds = request.MunicipiosIds,
                        Ufs = request.Ufs
                    };

                    var sindicatosLaborais = await _sindicatoLaboralFactory.CriarPorUsuario(sindicatoLaboralRequest);

                    sindicatosLaboraisIds = sindicatosLaborais is not null ?
                        sindicatosLaborais.Select(sindicatoLaboral => sindicatoLaboral.Id) :
                        null;
                }

                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@"
                    SELECT 
                        cs.data_referencia `data`,
                        CASE WHEN cs.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS `origem`,
                        `sd`.`inicio_mandatoe` AS `validade_inicial`,
                        `sd`.`termino_mandatoe` AS `validade_final`,
                        `se`.`sigla_sinde` AS `siglas_sindicatos_laborais`,
                        `se`.`id_sinde` AS `sindicato_laboral_id`,
                        `se`.`id_sinde` AS `sindicato_id`
                    FROM calendario_sindical_tb cs
                    INNER JOIN sind_emp se ON `se`.`id_sinde` = cs.chave_referencia
                    LEFT JOIN LATERAL (
        	            SELECT * FROM sind_diremp sdi 
        	            WHERE sdi.termino_mandatoe = cs.data_referencia AND sdi.sind_emp_id_sinde = se.id_sinde LIMIT 1
                    ) AS sd ON true
                    WHERE cs.tipo_evento = @tipoEvento
                ");

                parameters.Add(new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.VencimentoMandatoSindicalLaboral.Id));

                if (sindicatosLaboraisIds is not null && sindicatosLaboraisIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, sindicatosLaboraisIds, "se.id_sinde", parameters, ref parametersCount);
                }
                else
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds!, "se.id_sinde", parameters, ref parametersCount);
                }

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND cs.data_referencia BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND cs.data_referencia >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "cs.origem", parameters, ref parametersCount);
                }

                var query = _context.EventoCalendarioVencimentoMandatoLaboralVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioVencimentoMandatoViewModel
                            {
                                SindId = ec.SindId,
                                Data = ec.Data,
                                Origem = ec.Origem,
                                ValidadeInicial = ec.ValidadeInicial,
                                ValidadeFinal = ec.ValidadeFinal,
                                SiglaSindicato = ec.SiglasSindicatosLaborais,
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("trintidios")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioVencimentoMandatoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<EventoCalendarioVencimentoMandatoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterEventosTrintidiosAsync([FromQuery] EventoCalendarioRequest request)
        {
            var emailUsuario = _userInfoService.GetEmail();

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@"
                    SELECT * FROM evento_calendario_trintidio_vw vw
                    WHERE EXISTS (
                        SELECT * FROM documento_estabelecimento_tb det
                        INNER JOIN cliente_unidades cut ON cut.id_unidade = det.estabelecimento_id
                        INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                        WHERE CASE WHEN uat.nivel = 'Ineditta' THEN TRUE
                                   WHEN uat.nivel = 'Grupo Econômico' THEN uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                   ELSE JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade))
                              END
                              AND det.documento_id = vw.chave_referencia
                    )
                ");

                parameters.Add(new MySqlParameter("@email", emailUsuario));
                parameters.Add(new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.Trintidio.Id));

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND vw.data BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND vw.data >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "vw.origem", parameters, ref parametersCount);
                }

                if (request.NomesDocumentosIds is not null && request.NomesDocumentosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.NomesDocumentosIds, "vw.tipo_doc", parameters, ref parametersCount);
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds, "vw.sind_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds, "vw.sind_patronal", "id", parameters, ref parametersCount);
                }

                var query = _context.EventoCalendarioTrintidioVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioTrintidioViewModel
                            {
                                Data = ec.Data,
                                Origem = ec.Origem,
                                AtividadesEconomicas = ec.AtividadesEconomicas,
                                ValidadeInicial = ec.ValidadeInicial,
                                ValidadeFinal = ec.ValidadeFinal,
                                SindicatoLaboralId = ec.SindicatoLaboralId,
                                SindicatoPatronalId = ec.SindicatoPatronalId,
                                SiglasSindicatosLaborais = ec.SiglasSindicatosLaborais,
                                SiglasSindicatosPatronais = ec.SiglasSindicatosPatronais,
                                DataBase = ec.DataBase,
                                NomeDocumento = ec.NomeDocumento,
                                ChaveReferenciaId = ec.ChaveReferenciaId,
                                SindicatosLaborais = ec.SindicatosLaborais,
                                SindicatosPatronais = ec.SindicatosPatronais
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("eventos-clausulas")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoCalendarioClausulasViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<EventoCalendarioClausulasViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterEventosClausulasAsync([FromQuery] EventoCalendarioRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                IEnumerable<int>? sindeIds = null;
                IEnumerable<int>? sindpIds = null;
                if ((request.SindicatosLaboraisIds is null || !request.SindicatosLaboraisIds.Any()) || (request.SindicatosPatronaisIds is null || !request.SindicatosPatronaisIds.Any()))
                {
                    var parametersSind = new List<MySqlParameter>();
                    var parametersSindCount = 0;

                    var sindEmpregadosString = new StringBuilder(@"select sinde.*
	                                                        from sind_emp sinde
	                                                        where exists(select 1 
			                                                            from cliente_matriz as cmt 
                                                                        inner join usuario_adm uat on uat.email_usuario = @email
                                                                        LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
                                                                        LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                        LEFT join base_territorialsindemp AS bl 
                                                                            on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                            AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                                                        LEFT JOIN localizacao loc on bl.localizacao_id_localizacao1 = loc.id_localizacao
                                                                        LEFT join base_territorialsindpatro AS bp 
					                                                        on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                                                        AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
		                                                                 where 
		                                                                 case WHEN uat.nivel = @nivel then true 
                                                                         when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                         else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                                                                         and  sinde.id_sinde = bl.sind_empregados_id_sinde1");

                    var sindPatronaisString = new StringBuilder(@"select sindp.*
                                                        from sind_patr sindp
                                                        where exists(select 1 
			                                                        from cliente_matriz as cmt 
                                                                    inner join usuario_adm uat on uat.email_usuario = @email
                                                                    LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
                                                                    LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                                                    LEFT join base_territorialsindpatro AS bp 
                                                                            on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                                            AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
                                                                    LEFT JOIN localizacao loc on bp.localizacao_id_localizacao1 = loc.id_localizacao
                                                                    LEFT join base_territorialsindemp AS bl 
					                                                        on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                                                        AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
		                                                             where 
		                                                             case WHEN uat.nivel = @nivel then true 
                                                                          when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                     else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                                                                     and sindp.id_sindp = bp.sind_patronal_id_sindp");

                    parametersSind.Add(new MySqlParameter("@email", _userInfoService.GetEmail()!));
                    parametersSind.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
                    parametersSind.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                    if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parametersSind, ref parametersSindCount);
                    }

                    if (request.MatrizesIds != null && request.MatrizesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parametersSind, ref parametersSindCount);
                    }

                    if (request.EstabelecimentosIds != null && request.EstabelecimentosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.EstabelecimentosIds, "cut.id_unidade", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.EstabelecimentosIds, "cut.id_unidade", parametersSind, ref parametersSindCount);
                    }

                    if ((request.MunicipiosIds != null && request.MunicipiosIds.Any()) || (request.Ufs != null && request.Ufs.Any()))
                    {
                        if ((request.MunicipiosIds != null && request.MunicipiosIds.Any()) && (request.Ufs != null && request.Ufs.Any()))
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MunicipiosIds, "loc.id_localizacao", request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MunicipiosIds, "loc.id_localizacao", request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                        }
                        else if (request.MunicipiosIds != null && request.MunicipiosIds.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.MunicipiosIds, "loc.id_localizacao", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.MunicipiosIds, "loc.id_localizacao", parametersSind, ref parametersSindCount);
                        }
                        else if (request.Ufs != null && request.Ufs.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(sindPatronaisString, request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                            QueryBuilder.AppendListToQueryBuilder(sindEmpregadosString, request.Ufs, "loc.uf", parametersSind, ref parametersSindCount);
                        }
                    }

                    if (request.AtividadesEconomicasIds != null && request.AtividadesEconomicasIds.Any())
                    {
                        QueryBuilder.AppendListJsonToQueryBuilder(sindPatronaisString, request.AtividadesEconomicasIds, "cut.cnae_unidade", "id", parametersSind, ref parametersSindCount);
                        QueryBuilder.AppendListJsonToQueryBuilder(sindEmpregadosString, request.AtividadesEconomicasIds, "cut.cnae_unidade", "id", parametersSind, ref parametersSindCount);
                    }

                    sindPatronaisString.Append(')');
                    sindEmpregadosString.Append(')');

                    var querySinde = _context.SindEmps
                    .FromSqlRaw(sindEmpregadosString.ToString(), parametersSind.ToArray())
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

                    var querySindp = _context.SindPatrs
                    .FromSqlRaw(sindPatronaisString.ToString(), parametersSind.ToArray())
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

                    sindeIds = resultQuerySinde.Select(x => x.Id);
                    sindpIds = resultQuerySindp.Select(x => x.Id);
                }

                var parameters = new List<MySqlParameter>();
                var parametersCount = 0;

                var queryBuilder = new StringBuilder(@"
                    SELECT
                        cs.id id_evento,
                        cs.data_referencia AS `data_evento`,
                        CASE WHEN cs.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS `origem_evento`,
	                    cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional nome_evento_id,
	                    at2.nmtipoinformacaoadicional nome_evento,
	                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
                        REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais,
                        REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
	                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais,
	                    ec2.nome_clausula,
	                    gc.nome_grupo,
	                    ds.*,
	                    td.nome_doc nome_documento,
	                    td.idtipo_doc tipo_doc_id,
                        cgec2.clausula_geral_id_clau clausula_geral_id
                    FROM calendario_sindical_tb cs
                    LEFT JOIN clausula_geral_estrutura_clausula cgec2 ON cgec2.id_clausulageral_estrutura_clausula = cs.chave_referencia
                    LEFT JOIN doc_sind ds ON ds.id_doc = cgec2.doc_sind_id_doc
                    LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                    LEFT JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional = cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                    LEFT JOIN estrutura_clausula ec2 ON ec2.id_estruturaclausula = cgec2.nome_informacao
                    LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec2.grupo_clausula_idgrupo_clausula
                    WHERE cs.tipo_evento = @tipoEvento
                ");

                parameters.Add(new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.EventoClausula.Id));

                if (request.DataInicial.HasValue && request.DataFinal.HasValue)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    queryBuilder.Append(@" AND cs.data_referencia BETWEEN STR_TO_DATE(@validadeInicial, '%Y-%m-%d') AND STR_TO_DATE(@validadeFinal, '%Y-%m-%d')"
                    );

                    parameters.Add(new MySqlParameter("@validadeInicial", request.DataInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@validadeFinal", request.DataFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
                }
                else
                {
                    queryBuilder.Append(@" AND cs.data_referencia >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)");
                }

                if (request.Origem is not null && request.Origem.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.Origem, "cs.origem", parameters, ref parametersCount);
                }

                if (request.NomesDocumentosIds is not null && request.NomesDocumentosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.NomesDocumentosIds, "td.idtipo_doc", parameters, ref parametersCount);
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds, "ds.sind_laboral", "id", parameters, ref parametersCount);
                }
                else
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, sindeIds!, "ds.sind_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds, "ds.sind_patronal", "id", parameters, ref parametersCount);
                }
                else
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, sindpIds!, "ds.sind_patronal", "id", parameters, ref parametersCount);
                }

                var query = _context.EventoCalendarioDescontoPagamentoVencimentoVw.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray());

                var result = await query
                            .Select(ec => new EventoCalendarioClausulasViewModel
                            {
                                Id = ec.Id,
                                Data = ec.Data,
                                Origem = ec.Origem,
                                ClausulaGeralId = ec.ClausulaGeralId,
                                AtividadesEconomicas = ec.AtividadesEconomicas,
                                SiglasSindicatosLaborais = ec.SiglasSindicatosLaborais,
                                SiglasSindicatosPatronais = ec.SiglasSindicatosPatronais,
                                SindicatoLaboralId = ec.SindicatoLaboralId,
                                SindicatoPatronalId = ec.SindicatoPatronalId,
                                NomeDocumento = ec.NomeDocumento,
                                NomeClausula = ec.NomeClausula,
                                GrupoClausulas = ec.GrupoClausulas,
                                NomeEvento = ec.NomeEvento
                            }).ToDataTableResponseAsync(request);
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("{idEvento}/texto-clausulas")]
        public async ValueTask<IActionResult> ObterTextoDaClausulaPorEvento([FromRoute] int idEvento)
        {
            if (idEvento == 0)
            {
                return BadRequest("O idEvento passado na rota deve ser maior que 0");
            }
            var parameters = new Dictionary<string, object>();

            var queryBuilder = new StringBuilder(@"
                select cg.tex_clau from calendario_sindical_tb cst
                inner join clausula_geral_estrutura_clausula cgec on cgec.id_clausulageral_estrutura_clausula = cst.chave_referencia
                left join clausula_geral cg on cg.id_clau = cgec.clausula_geral_id_clau
                where cst.tipo_evento = 5 and cst.id = @idEvento
                group by cg.id_clau
            ");

            parameters.Add("idEvento", idEvento);

            var query = _context.SelectFromRawSqlAsync<string>(queryBuilder.ToString(), parameters);

            var result = await query;

            return Ok(result);
        }

        [HttpPost]
        [Route("calendarios-sindicais-usuario")]
        public async ValueTask<IActionResult> CriarCalendarioSindicalUsuario([FromBody] UpsertCalendarioSindicalUsuarioRequest request)
        {
            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [Route("agenda-eventos")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoUsuarioViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterAgendaEventosUsuario([FromQuery] EventoCalendarioRequest request)
        {
            var usuarioRequerente = await _usuarioRepository.ObterPorEmailAsync(_userInfoService.GetEmail()!);

            if (usuarioRequerente == null) return BadRequest("Usuario da requisição não encontrado");

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var dataInicial = request.DataInicial!.Value;
                var dataFinal = request.DataFinal!.Value;
                var tipoEventoAgendaId = TipoEventoCalendarioSindical.AgendaEventos.Id;


                var query = from calendarioSindical in _context.Eventos
                            join calendarioSindicalUsuario in _context.CalendariosSindicaisUsuario on calendarioSindical.ChaveReferenciaId equals calendarioSindicalUsuario.Id into _calendarioSindicalUsuario
                            from calendarioSindicalUsuario in _calendarioSindicalUsuario.DefaultIfEmpty()
                            join usuario in _context.UsuarioAdms on calendarioSindicalUsuario.IdUsuario equals usuario.Id into _usuario
                            from usuario in _usuario.DefaultIfEmpty()
                            where (usuario.Id == usuarioRequerente.Id ||
                                (usuario.GrupoEconomicoId == usuarioRequerente.GrupoEconomicoId && calendarioSindicalUsuario.Visivel)
                            )
                            && calendarioSindical.TipoEvento == tipoEventoAgendaId
                            && EF.Functions.DateDiffSecond(dataInicial, calendarioSindical.DataReferencia) >= 0
                            && EF.Functions.DateDiffSecond(calendarioSindical.DataReferencia, dataFinal) >= 0
                            select new EventoUsuarioViewModel
                            {
                                IdEvento = calendarioSindical.Id,
                                Data = calendarioSindical.DataReferencia,
                                Comentario = calendarioSindicalUsuario.Comentarios,
                                Local = calendarioSindicalUsuario.Local,
                                Recorrencia = calendarioSindicalUsuario.Recorrencia,
                                Titulo = calendarioSindicalUsuario.Titulo,
                                ValidadeRecorrencia = calendarioSindicalUsuario.ValidadeRecorrencia,
                                Visivel = calendarioSindicalUsuario.Visivel,
                            };

                var result = await query.ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [Route("tipos-subtipos")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EventoUsuarioViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTiposSubtipos([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = _context.TiposSubtiposVw.ToDataTableResponseAsync(request);
                return Ok(await result);
            }

            return BadRequestFromErrorMessage("Tipo de retorno não aceito");
        }
    }
}
