using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Auth;
using System.Text;
using Ineditta.Application.Usuarios.Entities;
using QueryBuilder = Ineditta.Repository.Extensions.QueryBuilder;
using Ineditta.BuildingBlocks.Core.Extensions;
using MySqlConnector;
using Ineditta.Application.AcompanhamentosCcts.UseCases.EnviarEmail;
using Ineditta.API.Services;
using Ineditta.API.Filters;
using Modulo = Ineditta.Application.ModulosEstaticos.Entities.Modulo;
using Ineditta.API.Builders.AcompanhamentosCcts;
using Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsFuturas;
using Ineditta.Application.Acompanhamentos.Ccts.UseCases.Upsert;
using Microsoft.AspNetCore.Components;
using Ineditta.Application.Acompanhamentos.Ccts.UseCases.AdicionarScript;
using Ineditta.Application.CctsFases.Entities;
using Microsoft.AspNetCore.Mvc;
using Route = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Ineditta.API.ViewModels.Shared.Requests;
using Ineditta.API.ViewModels.AcompanhamentosCct.Requests;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.NegociacoesFases;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.NegociacoesAbertas;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.DataTables;
using Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.Scripts;
using System.Text.Json;


namespace Ineditta.API.Controllers.V1.AcompanhamentosCcts
{
    [Route("v{version:apiVersion}/acompanhamentos-cct")]
    [ApiController]
    public class AcompanhamentoCctController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly HttpRequestItemService _httpRequestItemService;
        private readonly RelatorioAcompanhamentoCctBuilder _relatorioAcompanhamentoCctBuilder;
        public AcompanhamentoCctController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, HttpRequestItemService httpRequestItemService, RelatorioAcompanhamentoCctBuilder relatorioAcompanhamentoCctBuilder) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
            _httpRequestItemService = httpRequestItemService;
            _relatorioAcompanhamentoCctBuilder = relatorioAcompanhamentoCctBuilder;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AcompanhamentoCctDefaultDataTableViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NoContent();
            }

            var result = await (from acct in _context.AcompanhamentoCctInclusaoVw
                                select new AcompanhamentoCctDefaultDataTableViewModel
                                {
                                    Id = acct.Id,
                                    DataInicial = acct.DataInicial,
                                    DataFinal = acct.DataFinal,
                                    DataAlteracao = acct.DataAlteracao,
                                    Status = acct.Status,
                                    NomeUsuario = acct.NomeUsuario != null ? acct.NomeUsuario : string.Empty,
                                    Fase = acct.Fase,
                                    NomeDocumento = acct.NomeDocumento,
                                    ProxLigacao = acct.ProximaLigacao,
                                    DataBase = acct.DataBase,
                                    SiglaSindPatronal = acct.SiglaSinditoPatronal ?? string.Empty,
                                    UfSindPatronal = acct.UfSinditoPatronal ?? string.Empty,
                                    SiglaSindEmpregado = acct.SiglaSinditoLaboral ?? string.Empty,
                                    UfSindEmpregado = acct.UfSinditoLaboral ?? string.Empty,
                                    DescricaoSubClasse = acct.DescricaoSubClasse ?? string.Empty,
                                    Etiquetas = acct.Etiquetas
                                })
                            .ToDataTableResponseAsync(request);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("{id}/fases/respostas")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ScriptRespostasViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> ObterPerguntasRespostasFases([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>();

            var query = new StringBuilder(@"select jt.fase, jt.nomeUsuario, jt.horario, jt.respostas, j.fase as perguntas
                                            from acompanhamento_cct_tb act
                                            left join jfase j on true,
                                            json_table(act.scripts_salvos, '$[*]' COLUMNS(
                                                fase VARCHAR(255) PATH '$.Fase',
                                                horario VARCHAR(255) PATH '$.Horario',
                                                nomeUsuario VARCHAR(255) PATH '$.NomeUsuario',
                                                respostas json PATH '$.Respostas'
                                            )) as jt
                                            where JSON_UNQUOTE(JSON_EXTRACT(j.fase, '$.fase')) = jt.fase AND act.id = @id");

            parameters.Add("id", id);

            var result = await _context.SelectFromRawSqlAsync<ScriptRespostasViewModel>(query.ToString(), parameters);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("datatable-relatorio")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AcompanhamentoCctRelatorioDataTableViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterRelatorioAsync([FromQuery] AcompanhamentoCctRelatorioNegociacoesRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@fase", FasesCct.IndiceFaseArquivada));

                var sql = new StringBuilder(@"select DISTINCT vw.* from acompanhamento_cct_vw as vw
                                              inner join usuario_adm uat on uat.email_usuario = @email
                                              left join tipo_doc td on vw.tipo_documento_id = td.idtipo_doc
                                              where vw.fase_id <> @fase
                                              and exists (
	                                                select * from acompanhamento_cct_estabelecimento_tb acet
                                                    WHERE acet.acompanhamento_cct_id = vw.id");

                if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, "acet.grupo_economico_id", parameters, ref parametersCount);
                }

                if (request.EmpresasIds is not null && request.EmpresasIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.EmpresasIds, "acet.empresa_id", parameters, ref parametersCount);
                }

                if (request.EstabelecimentosIds is not null && request.EstabelecimentosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.EstabelecimentosIds, "acet.estabelecimento_id", parameters, ref parametersCount);
                }

                sql.Append(@") ");

                if (request.AtividadesEconomicasIds is not null && request.AtividadesEconomicasIds.Any())
                {
                    parameters.Add(new MySqlParameter("@atividadesEconomicasIds", JsonSerializer.Serialize(request.AtividadesEconomicasIds)));

                    sql.Append(@" and JSON_OVERLAPS(vw.ids_cnaes, @atividadesEconomicasIds) ");
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    sql.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_laboral_tb acslt
	                                inner join sind_emp se on acslt.sindicato_id = se.id_sinde
	                                WHERE acslt.acompanhamento_cct_id = vw.id");

                    QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosLaboraisIds, "acslt.sindicato_id", parameters, ref parametersCount);

                    sql.Append(@") ");
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    sql.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_patronal_tb acspt
	                                inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp
	                                WHERE acspt.acompanhamento_cct_id = vw.id");

                    QueryBuilder.AppendListToQueryBuilder(sql, request.SindicatosPatronaisIds, "acspt.sindicato_id", parameters, ref parametersCount);

                    sql.Append(@") ");
                }

                if (request.DatasBases is not null && request.DatasBases.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.DatasBases, "vw.data_base", parameters, ref parametersCount);
                }

                if (request.NomeDocumento is not null && request.NomeDocumento.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.NomeDocumento, "td.idtipo_doc", parameters, ref parametersCount);
                }

                if (request.TipoLocalizacao is not null && request.Localizacoes is not null && request.Localizacoes.Any())
                {
                    sql.Append(@" and exists (
	                                select 1 from acompanhamento_cct_localizacao_tb aclt
	                                inner join localizacao l on l.id_localizacao = aclt.localizacao_id
	                                where aclt.acompanhamento_cct_id = vw.id ");

                    if (request.TipoLocalizacao == "uf")
                    {
                        QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.uf", parameters, ref parametersCount);
                    }

                    if (request.TipoLocalizacao == "regiao")
                    {
                        QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.regiao", parameters, ref parametersCount);
                    }

                    if (request.TipoLocalizacao == "municipio")
                    {
                        QueryBuilder.AppendListToQueryBuilder(sql, request.Localizacoes, "l.municipio", parameters, ref parametersCount);
                    }

                    sql.Append(@") ");
                }

                if (request.Fases is not null && request.Fases.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(sql, request.Fases, "vw.fase_id", parameters, ref parametersCount);
                }

                if (request.DataProcessamentoInicial is not null && request.DataProcessamentoFinal is not null)
                {
#pragma warning disable CA1305 // Specify IFormatProvider
                    parameters.Add(new MySqlParameter("@dataProcessamentoInicial", request.DataProcessamentoInicial.Value.ToString("yyyy-MM-dd")));
                    parameters.Add(new MySqlParameter("@dataProcessamentoFinal", request.DataProcessamentoFinal.Value.ToString("yyyy-MM-dd")));

                    sql.Append(@" and vw.data_processamento >= @dataProcessamentoInicial AND vw.data_processamento <= @dataProcessamentoFinal");
#pragma warning restore CA1305 // Specify IFormatProvider
                }

                var result = await _context.AcompanhamentoCctVws
                                .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                .AsNoTracking()
                                .Select(acct => new AcompanhamentoCctRelatorioDataTableViewModel
                                {
                                    SiglasSindicatosLaborais = acct.SindicatosLaboraisSiglas,
                                    CnpjsLaborais = acct.SindicatosLaboraisCnpjs,
                                    UfsSindicatoLaborais = acct.SindicatosLaboraisUfs,
                                    SiglasSindicatoPatronais = acct.SindicatosPatronaisSiglas,
                                    CnpjsPatronais = acct.SindicatosPatronaisCnpjs,
                                    UfsSindicatoPatronais = acct.SindicatosPatronaisUfs,
                                    AtividadeEconomica = acct.AtividadesEconomicas,
                                    NomeDocumento = acct.NomeDocumento,
                                    DataBase = acct.DataBase,
                                    PeriodoInpc = acct.PeriodoAnterior,
                                    InpcReal = acct.DadoReal,
                                    Fase = acct.Fase,
                                    Observacoes = acct.ObservacoesGerais,
                                    DataProcessamento = acct.DataProcessamento
                                })
                                .ToDataTableResponseAsync(request);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            return NotAcceptable();
        }

        [HttpPost("relatorios-negociacoes")]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Octet)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterRelatorioNegociacoes([FromBody] AcompanhamentoCctRelatorioNegociacoesRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return Unauthorized();
            }

            var relatorio = await _relatorioAcompanhamentoCctBuilder.HandleAsync(usuario.GrupoEconomicoId ?? 0, request);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            var date = DateOnly.FromDateTime(DateTime.Now);

            var fileName = $"relatório_acompanhamento_cct_ineditta_{date.Day}_{date.Month}_{date.Year}";

            return DownloadExcel(fileName, relatorio.Value);
        }

        [HttpGet("informacoes-inicias")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(AcompanhamentoCctInformacoesIniciaisViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterInformacoesIniciaisAsync()
        {
            var abertasCount = await _context.AcompanhamentoCct
                .Where(a => a.FaseId != null && a.FaseId != FasesCct.IndiceFaseFechada && a.FaseId != FasesCct.IndiceFaseConcluida)
                .CountAsync();

            var dataIntervalo = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            var semmovCount = await _context.AcompanhamentoCct
                .Where(a => DateOnly.FromDateTime(EF.Property<DateTime>(a, "DataAlteracao")) <= dataIntervalo)
                .CountAsync();

            var ligacoesCount = await _context.AcompanhamentoCct
                .Where(a => a.ProximaLigacao == DateOnly.FromDateTime(DateTime.Today))
                .CountAsync();

            return Ok(new AcompanhamentoCctInformacoesIniciaisViewModel
            {
                NegociacoesEmAberto = abertasCount,
                SemMovimentacao30Dias = semmovCount,
                LigacoesDoDia = ligacoesCount,
            });
        }

        [HttpGet("tipos-docs")]
        [Produces(SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTiposDocsAsync([FromQuery] AcompanhamentoCctTiposDocumentosRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            if (request.PorUsuario)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "email", _userInfoService.GetEmail()! },
                    { "nivel", nameof(Nivel.Ineditta).ToString() },
                    { "nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()! }
                };

#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1311 // Specify a culture or use an invariant version
                var sql = new StringBuilder(@"SELECT td.nome_doc nome, td.idtipo_doc id from acompanhamento_cct_tb act
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        inner join tipo_doc td on td.idtipo_doc = act.tipo_documento_id
                                        where exists (
	                                        SELECT 1 FROM acompanhamento_cct_estabelecimento_tb acet
	                                        where acet.acompanhamento_cct_id = act.id
	                                        AND
	                                            CASE
	                                                WHEN uat.nivel = @nivel THEN true
	                                                WHEN uat.nivel = @nivelGrupoEconomico THEN acet.grupo_economico_id = uat.id_grupoecon
	                                                ELSE json_contains(uat.ids_fmge, cast(acet.estabelecimento_id as char))
	                                            END
                                        )
                                        and td.nome_doc is not null
                                        GROUP by td.nome_doc
                                        order by td.nome_doc;");

                var resultPorUsuario = (await _context.SelectFromRawSqlAsync<AcompanhamentoCctSelectTipoDocumentoViewModel>(sql.ToString(), parameters))
                                .Where(td =>
                                    td.Nome is not null && td.Nome.ToLower().Contains("convenção coletiva") ||
                                    td.Nome is not null && td.Nome.ToLower().Contains("especifica") ||
                                    td.Nome is not null && td.Nome.ToLower() == "acordo coletivo" ||
                                    td.Nome is not null && td.Nome.ToLower().Contains("termo aditivo de acordo coletivo") ||
                                    td.Nome is not null && td.Nome.ToLower().Contains("acordo coletivo es")
                                 )
                                .Select(td => new OptionModel<int>
                                {
                                    Id = td.Id,
                                    Description = td.Nome,
                                })
                                .ToList();
#pragma warning restore CA1311 // Specify a culture or use an invariant version
#pragma warning restore CA1304 // Specify CultureInfo

                if (resultPorUsuario == null)
                {
                    return NoContent();
                }

                return Ok(resultPorUsuario);
            }

#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1311 // Specify a culture or use an invariant version
            var result = await _context.TipoDocs
                            .AsNoTracking()
                            .Where(td =>
                                td.Nome.ToLower().Contains("convenção coletiva") ||
                                td.Nome.ToLower().Contains("especifica") ||
                                td.Nome.ToLower() == "acordo coletivo" ||
                                td.Nome.ToLower().Contains("termo aditivo de acordo coletivo") ||
                                td.Nome.ToLower().Contains("acordo coletivo específico")
                                )
                            .Select(td => new OptionModel<int>
                            {
                                Id = td.Id,
                                Description = td.Nome,
                            })
                            .ToListAsync();
#pragma warning restore CA1311 // Specify a culture or use an invariant version
#pragma warning restore CA1304 // Specify CultureInfo

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("datas-bases")]
        [Produces(SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterDataBaseAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>
            {
                { "email", _userInfoService.GetEmail()! },
                { "nivel", nameof(Nivel.Ineditta).ToString() },
                { "nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()! }
            };

            var sql = new StringBuilder(@"SELECT data_base description, data_base id from acompanhamento_cct_tb act
                                        inner join usuario_adm uat on uat.email_usuario = @email
	                                    WHERE exists (
	                                        SELECT 1 FROM acompanhamento_cct_estabelecimento_tb acet
                                            where acet.acompanhamento_cct_id = act.id
	                                        and
	                                            CASE
	                                                WHEN uat.nivel = @nivel THEN true
	                                                WHEN uat.nivel = @nivelGrupoEconomico THEN 
	        	                                        acet.grupo_economico_id = uat.id_grupoecon
	                                                ELSE json_contains(uat.ids_fmge, cast(acet.estabelecimento_id as char))
	                                            END
                                        )
                                        GROUP by data_base
                                        order by data_base;");

            var result = (await _context.SelectFromRawSqlAsync<OptionModel<string>>(sql.ToString(), parameters)).ToList();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(AcompanhamentoCctViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterAcompanhamentoPorIdAsync([FromRoute] int id)
        {
            var listaEmailsDestinatario = await _context.UsuarioAdms.Where(x => x.Tipo == "Cliente").Select(x => x.Email.Valor).ToListAsync();

            var query = $@"SELECT 
                            cct.id,
                            cct.data_inicial,
                            cct.data_final,
                            cct.data_alteracao,
                            acsts.id status_id,
                            acsts.descricao status,
                            cct.usuario_responsavel_id,
                            fc.id_fase fase_id,
                            fc.fase_negociacao fase,
                            cct.observacoes_gerais,
                            cct.tipo_documento_id,
                            cct.data_base,
                            cct.grupos_economicos_ids,
                            cct.empresas_ids,
                            cct.cnaes_ids,
                            cct.anotacoes,
                            ua.nome_usuario,
                            td.nome_doc nome_tipo_documento,
                            td.idtipo_doc id_tipo_documento,
                            GROUP_CONCAT(cc.descricao_subclasse separator ', ') atividades_economicas,
                            sindicatos_laborais.s sindicatos_laborais_json,
                            sindicatos_patronais.s sindicatos_patronais_json,
                            j.fase j_form,
                            cct.data_processamento,
                            localizacoes.l localizacoes_ids_json,
                            assuntos.s assuntos_json,
                            etiquetas.s etiquetas_json,
                            cct.validade_final
                        FROM 
                            acompanhamento_cct_tb cct
                        left join acompanhamento_cct_status_opcao_tb acsts ON cct.status = acsts.id 
                        left join fase_cct fc ON cct.fase_id = fc.id_fase
                        LEFT JOIN usuario_adm ua on cct.usuario_responsavel_id = ua.id_user
                        LEFT JOIN tipo_doc td on cct.tipo_documento_id = td.idtipo_doc
                        LEFT JOIN classe_cnae cc on JSON_CONTAINS(cct.cnaes_ids, cast(concat('""', cc.id_cnae, '""') as char))
                        LEFT JOIN jfase j ON fc.fase_negociacao = JSON_UNQUOTE(JSON_EXTRACT(j.fase, ""$.fase""))
                        LEFT JOIN lateral (
                            select JSON_ARRAYAGG(
                                JSON_OBJECT(
                                    'Id', se.id_sinde,
                                    'Sigla', se.sigla_sinde,
                                    'Uf', se.uf_sinde,
                                    'Municipio', se.municipio_sinde,
                                    'Email1', se.email1_sinde,
                                    'Email2', se.email2_sinde,
                                    'Email3', se.email3_sinde,
                                    'Site', se.site_sinde,
                                    'Denominacao', se.denominacao_sinde,
                                    'Cnpj', se.cnpj_sinde
                                    )
                                ) as s from acompanhamento_cct_sindicato_laboral_tb acslt
                            inner join sind_emp se on acslt.sindicato_id = se.id_sinde
                            where acslt.acompanhamento_cct_id = @id
                        ) as sindicatos_laborais on true
                        LEFT JOIN lateral (
                            select JSON_ARRAYAGG(
                                JSON_OBJECT(
                                    'Id', sp.id_sindp,
                                    'Sigla', sp.sigla_sp,
                                    'Uf', sp.uf_sp,
                                    'Municipio', sp.municipio_sp,
                                    'Email1', sp.email1_sp,
                                    'Email2', sp.email2_sp,
                                    'Email3', sp.email3_sp,
                                    'Site', sp.site_sp,
                                    'Denominacao', sp.denominacao_sp,
                                    'Cnpj', sp.cnpj_sp
                                    )
                                ) as s from acompanhamento_cct_sindicato_patronal_tb acslt
                            inner join sind_patr sp on acslt.sindicato_id = sp.id_sindp
                            where acslt.acompanhamento_cct_id = @id
                        ) as sindicatos_patronais on true
                        LEFT JOIN lateral (
                            select JSON_ARRAYAGG(JSON_OBJECT( 'Id', ec.id_estruturaclausula, 'Descricao', ec.nome_clausula)) as s
                            from acompanhamento_cct_assunto_tb acat
                            inner join estrutura_clausula ec on acat.estrutura_clausula_id = ec.id_estruturaclausula
                            where acat.acompanhamento_cct_id = @id
                        ) as assuntos on true
                        LEFT JOIN lateral (
                            select JSON_ARRAYAGG(aclt.localizacao_id) as l from acompanhamento_cct_localizacao_tb aclt
                            where aclt.acompanhamento_cct_id = @id
                        ) as localizacoes on true
                        LEFT JOIN lateral (
                            select JSON_ARRAYAGG(JSON_OBJECT( 'Id', aceot.id, 'Descricao', aceot.descricao)) as s
                            from acompanhamento_cct_etiqueta_tb acet
                            inner join acompanhamento_cct_etiqueta_opcao_tb aceot on acet.acompanhamento_cct_etiqueta_opcao_id = aceot.id
                            where acet.acompanhamento_cct_id = @id
                        ) as etiquetas on true
                        WHERE cct.id = @id";

            var result = (await _context.SelectFromRawSqlAsync<AcompanhamentoCctPorIdViewModel>(query, new Dictionary<string, object> { { "id", id } }))
                .SingleOrDefault();

            if (result == null)
            {
                return NotFound();
            }

            result.ListaEmailDestinatarioClientes = listaEmailsDestinatario;

            return Ok(result);
        }

        [HttpGet("usuarios-grupos/{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(AcompanhamentoCctUsuariosPorGrupoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterUsuariosAcompanhamentoPorIdAsync([FromRoute] int id, [FromQuery] ObterUsuarioPoGruposRequest request)
        {
            var parameters = new Dictionary<string, object>();

            StringBuilder query;

            if (request.GrupoEconomico is not null && request.GrupoEconomico.Value)
            {
                query = new StringBuilder(@"SELECT DISTINCT ua.email_usuario email
                                            FROM usuario_adm ua
                                            LEFT JOIN acompanhamento_cct_tb ac ON true
                                            LEFT JOIN cliente_grupo cg ON ua.id_grupoecon = cg.id_grupo_economico,
                                            json_table(ua.modulos_comercial, '$[*]' COLUMNS (id INT PATH '$.id', consultar varchar(1) path '$.Consultar')) uatmdt
                                            WHERE ac.id = @id
                                            AND ua.notifica_email = 1
                                            and uatmdt.id = @modulo
                                            and uatmdt.consultar = '1'
                                            and ua.nivel <> @nivel
                                            and JSON_CONTAINS(ac.grupos_economicos_ids, cast(ua.id_grupoecon as JSON), '$')");
            }
            else
            {
                query = new StringBuilder(@"select distinct uat.email_usuario email from usuario_adm uat
                                            inner join acompanhamento_cct_tb act on true
                                            left join lateral (
	                                            select cu.id_unidade, cu.cliente_grupo_id_grupo_economico from acompanhamento_cct_estabelecimento_tb acet
	                                            inner join cliente_unidades cu on acet.estabelecimento_id = cu.id_unidade
	                                            where acet.acompanhamento_cct_id = @id
                                            ) as cut on true,
                                            json_table(uat.modulos_comercial, '$[*]' COLUMNS (id INT PATH '$.id', consultar varchar(1) path '$.Consultar')) uatmdt
                                            where uat.nivel <> @nivel
                                            and case when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico 
                                                else json_contains(uat.ids_fmge, cast(cut.id_unidade as char))
                                                end 
                                            and act.id = @id
                                            and uat.notifica_email = 1
                                            and uatmdt.id = @modulo
                                            and uatmdt.consultar = '1'");
            }

            parameters.Add("id", id);
            parameters.Add("nivel", Nivel.Ineditta.ToString());
            parameters.Add("nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription() ?? "");
            parameters.Add("modulo", Modulo.Comercial.AcompanhamentoCCT.Id);

            var result = (await _context.SelectFromRawSqlAsync<AcompanhamentoCctUsuariosPorGrupoViewModel>(query.ToString(), parameters))
                            ?.ToList();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("futuras")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<AcompanhamentoCctFuturasVw>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterFuturasAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.AcompanhamentoCctFuturasVws
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }

        [HttpGet("negociacoes-fases")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<NegociacaoFaseViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterNegociacoesPorFaseAsync([FromQuery] FiltroHomeRequest request)
        {
            request ??= new FiltroHomeRequest();

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>();


            var sql = new StringBuilder(@"SELECT act.fase_id, max(fct.fase_negociacao) nome, COUNT(act.fase_id) quantidade 
                                            from acompanhamento_cct_tb act
                                            left join fase_cct fct on act.fase_id = fct.id_fase 
                                            where exists (
                                                select 1
                                                from acompanhamento_cct_estabelecimento_tb acet
                                                inner join usuario_adm uat on uat.email_usuario = @email
                                                inner join cliente_unidades cu on acet.estabelecimento_id = cu.id_unidade
                                                left join cnae_emp ce on ce.cliente_unidades_id_unidade = cu.id_unidade
                                                where case
                                                    when uat.nivel = @nivel then true
                                                    when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = acet.grupo_economico_id
                                                    else json_contains(uat.ids_fmge, json_array(cast(cu.id_unidade as char)))
                                                end
                                                and acet.acompanhamento_cct_id = act.id");

            parameters.Add("email", _userInfoService.GetEmail()!);
            parameters.Add("nivel", nameof(Nivel.Ineditta).ToString());
            parameters.Add("nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()!);
            parameters.Add("faseConcluida", FasesCct.IndiceFaseConcluida);
            parameters.Add("faseArquivada", FasesCct.IndiceFaseArquivada);

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.UnidadesIds, "cu.id_unidade", parameters);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.MatrizesIds, "cu.cliente_matriz_id_empresa", parameters);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", parameters);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.CnaesIds, "ce.classe_cnae_idclasse_cnae", parameters);
            }

            sql.Append(@")
                            and act.fase_id not in (@faseConcluida, @faseArquivada)
                            group by 1");


            return Ok(await _context.SelectFromRawSqlAsync<NegociacaoFaseViewModel>(sql.ToString(), parameters));
        }

        [HttpGet("negociacoes-fases-ufs")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<NegociacaoUfViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterNegociacoesPorUfAsync([FromQuery] FiltroHomeRequest request)
        {
            request ??= new FiltroHomeRequest();

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>();


            var sql = new StringBuilder(@"SELECT acompanhamento_cct_id id, se.uf_sinde uf from acompanhamento_cct_sindicato_laboral_tb acslt
                                        inner join acompanhamento_cct_tb cct on acslt.acompanhamento_cct_id = cct.id
                                        left join fase_cct fc on cct.fase_id = fc.id_fase
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        inner join sind_emp se on acslt.sindicato_id = se.id_sinde
                                        inner join lateral (
                                                select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                left join cliente_unidades cu on acet.estabelecimento_id = cu.id_unidade
                                                left join cnae_emp ce on ce.cliente_unidades_id_unidade = cu.id_unidade
                                                where acet.acompanhamento_cct_id = acslt.acompanhamento_cct_id
                                                and case
                                                    when uat.nivel = @nivel then true
                                                    when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cu.cliente_grupo_id_grupo_economico
                                                    else json_contains(uat.ids_fmge, json_array(cast(cu.id_unidade as char)))
                                                end");

            parameters.Add("email", _userInfoService.GetEmail()!);
            parameters.Add("nivel", nameof(Nivel.Ineditta).ToString());
            parameters.Add("nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()!);
            parameters.Add("faseConcluida", FasesCct.IndiceFaseConcluida);
            parameters.Add("faseArquivada", FasesCct.IndiceFaseArquivada);

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.UnidadesIds, "cu.id_unidade", parameters);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.MatrizesIds, "cu.cliente_matriz_id_empresa", parameters);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", parameters);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.CnaesIds, "ce.classe_cnae_idclasse_cnae", parameters);
            }

            sql.Append(@") as c on true
                            where cct.fase_id not in (@faseConcluida, @faseArquivada)
                            order by acompanhamento_cct_id;");

            var result = await _context.SelectFromRawSqlAsync<NegociacaoUfViewModel>(sql.ToString(), parameters);

            if (result == null)
            {
                return NoContent();
            }

            List<NegociacoesPorUfViewModel> counter = new();

            foreach (var item in result)
            {
#pragma warning disable S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
                if (counter.Any(t => t.Uf == item.Uf))
                {
                    counter = counter.Select(counterItem =>
                    {
                        if (counterItem.Uf == item.Uf)
                        {
                            if (counterItem.Id.Contains(item.Id))
                            {
                                return counterItem;
                            }

                            counterItem.Id.Add(item.Id);
                            counterItem.Quantidade += 1;

                            return counterItem;
                        }

                        return counterItem;
                    }).ToList();
                }
                else
                {
                    counter.Add(new NegociacoesPorUfViewModel
                    {
                        Id = new List<long>()
                        {
                            item.Id
                        },
                        Quantidade = 1,
                        Uf = item.Uf
                    });
                }
#pragma warning restore S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
            }

            return Ok(counter);
        }

        [HttpGet("negociacoes-abertas")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<NegociacaoAbertaViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterNegociacoesAbertasAsync([FromQuery] FiltroHomeRequest request)
        {
            request ??= new FiltroHomeRequest();

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
            parameters.Add(new MySqlParameter("@faseConcluida", FasesCct.IndiceFaseConcluida));
            parameters.Add(new MySqlParameter("@faseArquivada", FasesCct.IndiceFaseArquivada));
            parameters.Add(new MySqlParameter("@tipoFaseCct", FasesCct.IndiceTipoFaseCct));

            var sql = new StringBuilder(@"select SUBSTR(cct.data_base, 1, 3) nome_mes, count(distinct cct.id) quantidade
                                        from acompanhamento_cct_tb cct 
                                        left join fase_cct fc ON cct.fase_id = fc.id_fase
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        inner join lateral (
                                                select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                inner join cliente_unidades cu on acet.estabelecimento_id = cu.id_unidade
                                                left join cnae_emp ce on ce.cliente_unidades_id_unidade = cu.id_unidade
                                                where acet.acompanhamento_cct_id = cct.id
                                                and case
                                                    when uat.nivel = @nivel then true
                                                    when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cu.cliente_grupo_id_grupo_economico
                                                    else json_contains(uat.ids_fmge, json_array(cast(cu.id_unidade as char)))
                                                end
                                        
                                    ");

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.UnidadesIds, "cu.id_unidade", parameters, ref parametersCount);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.MatrizesIds, "cu.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.CnaesIds, "ce.classe_cnae_idclasse_cnae", parameters, ref parametersCount);
            }

            if (!string.IsNullOrEmpty(request.AnoNeg))
            {
                sql.Append(" and cct.data_base like CONCAT('%', @anoNeg, '%')");
                parameters.Add(new MySqlParameter("@anoNeg", request.AnoNeg));
            }

            if (request.NomeDocNegId > 0)
            {
                sql.Append(" and cct.tipo_documento_id = @tipoDoc");
                parameters.Add(new MySqlParameter("@tipoDoc", request.NomeDocNegId));
            }

            sql.Append(@") as c on true
                        where fc.id_fase not in (@faseConcluida, @faseArquivada)
                        and fc.tipo_fase = @tipoFaseCct ");


            if (!string.IsNullOrEmpty(request.FaseDocNeg))
            {
                sql.Append(" and fc.id_fase = @faseNeg ");
                parameters.Add(new MySqlParameter("@faseNeg", request.FaseDocNeg));
            }

            sql.Append(@" GROUP BY 1");

            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();

            foreach (MySqlParameter parameter in parameters)
            {
                if (parameter != null && parameter.Value != null)
                {
                    parameterDictionary[parameter.ParameterName] = parameter.Value;
                }
            }

            var negociacoesAbertas = (await _context.SelectFromRawSqlAsync<NegociacaoAbertaViewModel>(sql.ToString(), parameterDictionary)).ToList()
                                    ?? new List<NegociacaoAbertaViewModel>();

            foreach (var month in DateTimeExtension.GetAbbreviatedMonthsInYear())
            {
                if (!negociacoesAbertas.Exists(nat => nat.NomeMes.ToUpperInvariant() == month.Value.ToUpperInvariant()))
                {
                    negociacoesAbertas.Add(new NegociacaoAbertaViewModel { Mes = month.Key, NomeMes = month.Value.ToUpperInvariant() });
                }

                negociacoesAbertas.Single(nat => nat.NomeMes.ToUpperInvariant() == month.Value.ToUpperInvariant()).Mes = month.Key;
            }

            return Ok(negociacoesAbertas);
        }

        [HttpGet("nome-documento-filtro")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterNomesDocumentosAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var result = await (from cct in _context.AcompanhamentoCct
                                join td in _context.TipoDocs on cct.TipoDocumentoId equals td.Id into _td
                                from td in _td.DefaultIfEmpty()
                                select new OptionModel<int>
                                {
                                    Id = td.Id,
                                    Description = td.Nome
                                })
                               .Distinct()
                               .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("fases-filtro")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterFasesFiltroAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>
                {
                    { "email", _userInfoService.GetEmail()! },
                    { "nivel", nameof(Nivel.Ineditta).ToString() },
                    { "nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()! },
                    { "fase", FasesCct.IndiceFaseArquivada }
                };

#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1311 // Specify a culture or use an invariant version
            var sql = new StringBuilder(@"SELECT fc.fase_negociacao description, fc.id_fase id from acompanhamento_cct_tb act
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        inner join fase_cct fc on act.fase_id = fc.id_fase
                                        where exists (
	                                        SELECT 1 FROM acompanhamento_cct_estabelecimento_tb acet
	                                        where acet.acompanhamento_cct_id = act.id
	                                        AND
	                                            CASE
	                                                WHEN uat.nivel = @nivel THEN true
	                                                WHEN uat.nivel = @nivelGrupoEconomico THEN acet.grupo_economico_id = uat.id_grupoecon
	                                                ELSE json_contains(uat.ids_fmge, cast(acet.estabelecimento_id as char))
	                                            END
                                        )
                                        and fc.fase_negociacao is not null
                                        and fase_id <> @fase
                                        GROUP by 1
                                        order by fc.fase_negociacao;");

            var result = (await _context.SelectFromRawSqlAsync<AcompanhamentoCctSelectFasesViewModel>(sql.ToString(), parameters))
                            .Select(td => new OptionModel<int>
                            {
                                Id = td.Id,
                                Description = td.Description,
                            })
                            .ToList();
#pragma warning restore CA1311 // Specify a culture or use an invariant version
#pragma warning restore CA1304 // Specify CultureInfo

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("ano-base-filtro")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> ObterAnosBasesFiltroAsync()
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var query = @"select * from (select DISTINCT SUBSTR(data_base, 5) ano
                                from acompanhamento_cct_tb cct) x
                                where x.ano <> ''";

                var result = await _context.SelectFromRawSqlAsync<dynamic>(query, new Dictionary<string, object>());

                if (result != null)
                {
                    return Ok(result.Select(x => new OptionModel<int>
                    {
                        Id = int.Parse(x.ano),
                        Description = x.ano,
                    }).OrderByDescending(x => x.Id));
                }

                return NoContent();
            }

            return NotAcceptable();
        }

        [HttpPatch("scripts")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> SalvarScript([FromBody] AdicionarScriptRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }

        [HttpPost("email-contato")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> EnviarEmailBoasVindas([FromBody] EnviarEmailContatoRequest request, [FromHeader(Name = "X-request-ID")] Guid requestId)
        {
            request.IdempotentToken = requestId;

            return await Dispatch(request);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<IActionResult> Criar([FromBody] UpsertAcompanhamentoCctRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertAcompanhamentoCctRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }


        [HttpGet("localidades")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterLocalidadesAsync([FromQuery] AcompanhamentoCctRelatorioNegociacoesSelectLocalidadeRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NoContent();
            }

            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta).ToString()));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
            parameters.Add(new MySqlParameter("@fase", FasesCct.IndiceFaseArquivada));

            var query = new StringBuilder(@"SELECT ");

            if (request.TipoLocalidade == "uf")
            {
                query.Append(@"l.uf id, l.uf description ");
            }
            else if (request.TipoLocalidade == "municipio")
            {
                query.Append(@"l.municipio id, l.municipio description ");
            }
            else if (request.TipoLocalidade == "regiao")
            {
                query.Append(@"l.regiao id, l.regiao description ");
            }
            else
            {
                query.Append(@"concat(l.uf, '/', l.municipio) id, concat(l.uf, '/', l.municipio) description ");
            }

            query.Append(@" from acompanhamento_cct_vw vw
                inner join usuario_adm uat on uat.email_usuario = @email
                left join tipo_doc td on vw.tipo_documento_id = td.idtipo_doc
                inner join acompanhamento_cct_localizacao_tb aclt on aclt.acompanhamento_cct_id = vw.id
                inner join localizacao l on aclt.localizacao_id = l.id_localizacao
                where vw.fase_id <> 8
                and exists (
                      select * from acompanhamento_cct_estabelecimento_tb acet
                      WHERE acet.acompanhamento_cct_id = vw.id");

            if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.GruposEconomicosIds, "acet.grupo_economico_id", parameters, ref parametersCount);
            }

            if (request.EmpresasIds is not null && request.EmpresasIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.EmpresasIds, "acet.empresa_id", parameters, ref parametersCount);
            }

            if (request.EstabelecimentosIds is not null && request.EstabelecimentosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.EstabelecimentosIds, "acet.estabelecimento_id", parameters, ref parametersCount);
            }

            query.Append(@") ");

            if (request.AtividadesEconomicasIds is not null && request.AtividadesEconomicasIds.Any())
            {
                parameters.Add(new MySqlParameter("@atividadesEconomicasIds", JsonSerializer.Serialize(request.AtividadesEconomicasIds)));

                query.Append(@" and JSON_OVERLAPS(vw.ids_cnaes, @atividadesEconomicasIds) ");
            }

            if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
            {
                query.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_laboral_tb acslt
	                                inner join sind_emp se on acslt.sindicato_id = se.id_sinde
	                                WHERE acslt.acompanhamento_cct_id = vw.id");

                QueryBuilder.AppendListToQueryBuilder(query, request.SindicatosLaboraisIds, "acslt.sindicato_id", parameters, ref parametersCount);

                query.Append(@") ");
            }

            if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
            {
                query.Append(@" and exists (
	                                SELECT 1 from acompanhamento_cct_sindicato_patronal_tb acspt
	                                inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp
	                                WHERE acspt.acompanhamento_cct_id = vw.id");

                QueryBuilder.AppendListToQueryBuilder(query, request.SindicatosPatronaisIds, "acspt.sindicato_id", parameters, ref parametersCount);

                query.Append(@") ");
            }

            if (request.DatasBases is not null && request.DatasBases.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.DatasBases, "vw.data_base", parameters, ref parametersCount);
            }

            if (request.NomeDocumento is not null && request.NomeDocumento.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.NomeDocumento, "td.idtipo_doc", parameters, ref parametersCount);
            }

            if (request.Fases is not null && request.Fases.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.Fases, "vw.fase_id", parameters, ref parametersCount);
            }

            if (request.DataProcessamentoInicial is not null && request.DataProcessamentoFinal is not null)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                parameters.Add(new MySqlParameter("@dataProcessamentoInicial", request.DataProcessamentoInicial.Value.ToString("yyyy-MM-dd")));
                parameters.Add(new MySqlParameter("@dataProcessamentoFinal", request.DataProcessamentoFinal.Value.ToString("yyyy-MM-dd")));

                query.Append(@" and vw.data_processamento >= @dataProcessamentoInicial AND vw.data_processamento <= @dataProcessamentoFinal");
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            query.Append(@" group by 2
                ORDER BY l.id_localizacao;");

            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();

            foreach (MySqlParameter parameter in parameters)
            {
                if (parameter != null && parameter.Value != null)
                {
                    parameterDictionary[parameter.ParameterName] = parameter.Value;
                }
            }

            var result = await _context.SelectFromRawSqlAsync<OptionModel<string>>(query.ToString(), parameterDictionary);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}