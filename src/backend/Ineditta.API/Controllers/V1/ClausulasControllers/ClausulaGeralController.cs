using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using System.Net.Mime;
using Ineditta.API.Filters;
using CSharpFunctionalExtensions;
using Ineditta.API.Factories.Clausulas;
using Ineditta.API.Builders.Clausulas;
using Ineditta.API.ViewModels.ClausulasGerais.Requests;
using Ineditta.API.ViewModels.ClausulasGerais.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.Clausulas.Gerais.UseCases.EnviarEmailClausulasAprovadas;
using Ineditta.Repository.Clausulas.Geral.Views.ClausulaGeral;
using Ineditta.Application.Clausulas.Gerais.UseCases.Upsert;
using Ineditta.Application.Clausulas.Gerais.UseCases.Aprovar;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.Clausulas.Gerais.UseCases.Resumir;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.Application.Clausulas.Gerais.UseCases.AtualizarTextoResumido;
using Ineditta.Application.Clausulas.Gerais.UseCases.Liberar;
using MySqlConnector;
using System.Text;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Repository.Extensions;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.Repository.Clausulas.Geral.Views.Clausula;
using Ineditta.API.Services;
using Ineditta.API.ViewModels.Clausulas.ViewModels;
using Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel;
using Ineditta.API.Converters.ViewModels.ClausulasGerais;
using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.API.ViewModels.ClausulasClientes.ViewModels;

namespace Ineditta.API.Controllers.V1.ClausulasControllers
{
    [Route("v{version:apiVersion}/clausulas-gerais")]
    [ApiController]
    public class ClausulaGeralController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly DocumentoSindicalClausulaFactory _documentoSindicalClausulaFactory;
        private readonly DocumentoSindicalClausulaPorIdFactory _documentoSindicalClausulaPorIdFactory;
        private readonly RelatorioClausulaBuilder _relatorioClausulaBuilder;
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly RelatorioBuscaRapidaPdfBuilder _relatorioBuscaRapidaPdfBuilder;
        private readonly HttpRequestItemService _httpRequestItemService;

        public ClausulaGeralController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, DocumentoSindicalClausulaFactory documentoSindicalClausulaFactory, DocumentoSindicalClausulaPorIdFactory documentoSindicalClausulaPorIdFactory, RelatorioClausulaBuilder relatorioClausulaBuilder, IClausulaGeralRepository clausulaGeralRepository, IUserInfoService userInfoService, RelatorioBuscaRapidaPdfBuilder relatorioBuscaRapidaPdfBuilder, HttpRequestItemService httpRequestItemService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _documentoSindicalClausulaFactory = documentoSindicalClausulaFactory;
            _documentoSindicalClausulaPorIdFactory = documentoSindicalClausulaPorIdFactory;
            _relatorioClausulaBuilder = relatorioClausulaBuilder;
            _clausulaGeralRepository = clausulaGeralRepository;
            _userInfoService = userInfoService;
            _relatorioBuscaRapidaPdfBuilder = relatorioBuscaRapidaPdfBuilder;
            _httpRequestItemService = httpRequestItemService;
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType, DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ClausulaGeralVw>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodos([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var result = await (from ect in _context.EstruturaClausula
                                    where _context.ClausulaGerals.Any(cg => cg.EstruturaClausulaId == ect.Id)
                                    select new OptionModel<int>
                                    {
                                        Id = ect.Id,
                                        Description = ect.Nome
                                    })
                                    .ToListAsync();

                return Ok(result);
            }
            else if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await _context.ClausulaGeraisVw
                    .ToDataTableResponseAsync(request);

                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("documentos/{id}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulasGeraisPorDocumentoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ClausulasGeraisPorDocumentoViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterPorTipoDocumento([FromRoute] int id, [FromQuery] DataTableRequest request)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            var query = from cg in _context.ClausulaGerals
                        join cgec in _context.InformacaoAdicionalSisap on cg.Id equals cgec.Id into _cgpe
                        from cgec in _cgpe.DefaultIfEmpty()
                        join ec in _context.EstruturaClausula on cg.EstruturaClausulaId equals ec.Id into _ec
                        from ec in _ec.DefaultIfEmpty()
                        join gc in _context.GrupoClausula on ec.GrupoClausulaId equals gc.Id into _gc
                        from gc in _gc.DefaultIfEmpty()
                        join ds in _context.DocSinds on cg.DocumentoSindicalId equals ds.Id into _ds
                        from ds in _ds.DefaultIfEmpty()
                        join us in _context.UsuarioAdms on cg.ResponsavelProcessamento equals us.Id into _us
                        from us in _us.DefaultIfEmpty()
                        join usa in _context.UsuarioAdms on (long?)cg.UsuarioAprovadorId equals usa.Id into _usa
                        from usa in _usa.DefaultIfEmpty()
                        where cg.DocumentoSindicalId == id && cg.ConstaNoDocumento
                        select new ClausulasGeraisPorDocumentoViewModel
                        {
                            Id = cg.Id,
                            IdDocumentoSindical = cg.DocumentoSindicalId,
                            NomeGrupo = gc.Nome,
                            NomeClausula = ec == null ? default : ec.Nome,
                            DataProcessamento = cg.DataProcessamento,
                            Numero = cg.Numero,
                            NomeAprovador = usa == null ? default : usa.Nome,
                            DataAprovacao = cg.DataAprovacao,
                            DataScrap = ds == null ? default : ds.DataScrap,
                            NomeResponsavelProcessamento = us == null ? default : us.Nome,
                            Resumivel = ec.Resumivel ? "Sim" : "Não",
                            ResumoStatus = cg.ResumoStatus == ResumoStatus.NaoResumida ? "Não Resumida" : cg.ResumoStatus == ResumoStatus.Resumindo ? "Resumindo" : "Resumido"
                        };
#pragma warning restore S3358 // Ternary operators should not be nested

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var resultDatatable = await query.ToDataTableResponseAsync(request);

                if (resultDatatable == null) return NoContent();

                return Ok(resultDatatable);
            }

            var result = await query.ToListAsync();

            if (result == null) return NoContent();

            return Ok(result);
        }

        [HttpGet("documentos/{id}/informacoes-adicionais")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<DocumentoSindicalClausulaVw>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterPorDocumentoInformacoesAdicionais([FromRoute] int id)
        {
            var result = await _documentoSindicalClausulaFactory.CriarAsync(id);

            return result == null || !result.Any() ? NoContent() : Ok(result.OrderBy(p => p.Numero));
        }

        [HttpGet("informacoes-adicionais/{idClausula}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<DocumentoSindicalClausulaVw>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterInformacoesAdicionaisPorClausulaId([FromRoute] int idClausula)
        {
            var result = await _documentoSindicalClausulaPorIdFactory.CriarAsync(idClausula);

            return result == null || !result.Any() ? NoContent() : Ok(result.OrderBy(p => p.Numero));
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulasGeraisViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterPorId([FromRoute] int id)
        {
            var result = await (from cg in _context.ClausulaGerals
                                join ec in _context.EstruturaClausula on cg.EstruturaClausulaId equals ec.Id into _ec
                                from ec in _ec.DefaultIfEmpty()
                                join ds in _context.DocSinds on cg.DocumentoSindicalId equals ds.Id into _ds
                                from ds in _ds.DefaultIfEmpty()
                                join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                from td in _td.DefaultIfEmpty()
                                join ast in _context.Assuntos on cg.AssuntoId equals ast.Idassunto into _ast
                                from ast in _ast.DefaultIfEmpty()
                                join sn in _context.Sinonimos on cg.SinonimoId equals sn.Id into _sn
                                from sn in _sn.DefaultIfEmpty()
                                where cg.Id == id
                                select new ClausulasGeraisViewModel
                                {
                                    Id = cg.Id,
                                    Texto = cg.Texto ?? string.Empty,
                                    IdEstruturaClausula = ec.Id,
                                    Nome = ec.Nome,
                                    DataAprovacao = cg.DataAprovacao,
                                    Aprovador = cg.UsuarioAprovadorId,
                                    Numero = cg.Numero,
                                    Documento = new DocumentoViewModel
                                    {
                                        Id = ds.Id,
                                        Nome = td.Nome,
                                        ValidadeInicial = ds.DataValidadeInicial,
                                        ValidadeFinal = ds.DataValidadeFinal,
                                    },
                                    Assunto = new AssuntoViewModel
                                    {
                                        Id = ast.Idassunto,
                                        Descricao = ast.Assunto1
                                    },
                                    Sinonimo = sn != null
                                    ? new OptionModel<int>
                                    {
                                        Id = sn.Id,
                                        Description = sn.Nome
                                    }
                                    : null,
                                    TextoResumido = cg.TextoResumido
                                })
                                .SingleOrDefaultAsync();

            if (result == null) return NoContent();

            return Ok(result);
        }

        [HttpPost("relatorio-busca-rapida")]
        [Produces(MediaTypeNames.Application.Pdf, MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(File), MediaTypeNames.Application.Pdf)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(File), MediaTypeNames.Application.Octet)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> Criar([FromBody] ClausulaGeralRelarotioBuscaRapidaRequest request)
        {
            if (Request.Headers.Accept == MediaTypeNames.Application.Pdf)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"select vw.* from clausulas_vw vw
                                        INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                        WHERE  vw.clausula_geral_liberada = 'S'
                                        and
                                            case when uat.nivel = @nivel then true 
                                                    when uat.nivel = @nivelGrupoEconomico then 
                                                        JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                                WHEN uat.nivel = @nivelMatriz THEN
                                                        JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""m"": ', CAST((
                                                            SELECT cut.cliente_matriz_id_empresa FROM cliente_unidades cut WHERE cut.id_unidade IN (
                                                                SELECT jt.ids FROM JSON_TABLE(uat.ids_fmge, '$[*]' COLUMNS (ids INT PATH '$')) as jt
                                                            ) LIMIT 1
                                                        ) AS CHAR), '}}'))
                                                    else JSON_CONTAINS(vw.documento_estabelecimento , CONCAT('{{""u"": ', CAST((JSON_EXTRACT(uat.ids_fmge, '$[0]')) AS CHAR), '}}'))
                                        end");

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

                if (request.ClausulasIds != null && request.ClausulasIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.ClausulasIds, "vw.clausula_id", parameters, ref parametersCount);
                }

                if (request.DataProcessamentoInicial.HasValue && request.DataProcessamentoFinal.HasValue)
                {
                    parameters.Add(new MySqlParameter("@dataProcessamentoInicial", request.DataProcessamentoInicial));
                    parameters.Add(new MySqlParameter("@dataProcessamentoFinal", request.DataProcessamentoFinal));

                    query.Append(@" AND vw.data_processamento_documento >= @dataProcessamentoInicial && vw.data_processamento_documento <= @dataProcessamentoFinal ");
                }

                query.Append(@" order by vw.documento_id, vw.regiao, vw.estrutura_clausula_id ");

                var clausulas = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .ToArrayAsync();
                if (clausulas == null)
                {
                    return NoContent();
                }

                var pdf = _relatorioBuscaRapidaPdfBuilder.Criar(clausulas.ToList());

                if (pdf.IsFailure)
                {
                    return NoContent();
                }

                return File(pdf.Value, "application/pdf", "relatorio_busca_rapida.pdf");
            }

            if (Request.Headers.Accept == MediaTypeNames.Application.Octet)
            {
                var usuario = _httpRequestItemService.ObterUsuario();

                if (usuario == null)
                {
                    return NoContent();
                }

                var parameters = new Dictionary<string, object> {
                    { "@userEmail", usuario.Email.Valor }
                };

                var query = new StringBuilder(@"
                    WITH documentos as (
	                    SELECT * from doc_sind ds
	                    left join lateral (
		                    SELECT cg.doc_sind_id_documento id from clausula_geral cg
                            inner join estrutura_clausula ec on cg.estrutura_id_estruturaclausula = ec.id_estruturaclausula
                            WHERE ec.resumivel = 1
                            and cg.aprovado = 'sim'
                            and cg.liberado = 'S'
                            and data_processamento_documento is not null");

                if (request.ClausulasIds is not null && request.ClausulasIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.ClausulasIds, "cg.id_clau", parameters);
                }

                query.Append(@"
		                    GROUP by cg.estrutura_id_estruturaclausula, cg.texto_resumido
	                    ) doc on true
	                    where doc.id = ds.id_doc
	                    and ds.data_liberacao_clausulas is not null
	                    group by ds.id_doc
                    )

                    select
                        ds.id_doc documento_id,
                        unidades.codigos_sindicato_cliente codigos_sindicato_cliente,
                        unidades.codigos codigos_unidades,
                        unidades.cnpjs cnpjs_unidades,
                        unidades.ufs ufs_unidades,
                        unidades.municipios municipios_unidades,
                        sindicatos_laborais.siglas siglas_sindicatos_laborais,
                        sindicatos_laborais.cnpjs cnpjs_sindicatos_laborais,
                        sindicatos_laborais.denominacoes denominacoes_sindicatos_laborais,
                        sindicatos_patronais.siglas siglas_sindicatos_patronais,
                        sindicatos_patronais.cnpjs cnpjs_sindicatos_patronais,
                        sindicatos_patronais.denominacoes denominacoes_sindicatos_patronais,
                        ds.data_liberacao_clausulas data_liberacao_clausulas,
                        td.nome_doc nome_documento,
                        ds.cnae_doc atividades_economicas_documento_string,
                        ds.validade_inicial validade_inicial_documento,
                        ds.validade_final validade_final_documento,
                        ds.database_doc database_documento,
                        ds.data_assinatura data_assinatura_documento,
                        ds.abrangencia abrangencia_documento_string,
                        ec.nome_clausula nome_clausula,
                        gc.nome_grupo nome_grupo_clausula,
                        cg.numero_clausula numero_clausula,
                        cg.texto_resumido texto_clausula,
                        cg.id_clau clausula_id,
                        cg.estrutura_id_estruturaclausula estrutura_clausula_id
                    from clausula_geral cg
                    inner join documentos ds on true
                    LEFT JOIN estrutura_clausula ec ON ec.id_estruturaclausula = cg.estrutura_id_estruturaclausula
                    LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula
                    LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc 
                    LEFT JOIN LATERAL (
                        SELECT 
                            GROUP_CONCAT(DISTINCT cut.codigo_unidade separator ', ') AS codigos, 
                            GROUP_CONCAT(DISTINCT cut.cnpj_unidade separator ', ') AS cnpjs,
                            GROUP_CONCAT(DISTINCT cut.cod_sindcliente separator ', ') AS codigos_sindicato_cliente,
                            GROUP_CONCAT(DISTINCT l.uf separator ', ') AS ufs,
                            GROUP_CONCAT(DISTINCT l.municipio separator ', ') AS municipios
                        FROM cliente_unidades cut
                        INNER JOIN usuario_adm ua on ua.email_usuario = 'bruno@threeo.com.br'
                        INNER JOIN doc_sind dsindt2 on ds.id_doc = dsindt2.id_doc
                        LEFT JOIN localizacao l on l.id_localizacao = cut.localizacao_id_localizacao,
                        json_table(dsindt2.cliente_estabelecimento, '$[*].u' columns (value int path '$')) as cus
                        WHERE cus.value = cut.id_unidade AND
                              CASE WHEN ua.nivel = 'Ineditta' THEN TRUE
                                   WHEN ua.nivel = 'Grupo Econômico' THEN cut.cliente_grupo_id_grupo_economico = ua.id_grupoecon
                                   ELSE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
                              END
                    ) unidades ON true
                    LEFT JOIN LATERAL (
                    SELECT 
                            GROUP_CONCAT(sp.sigla_sp separator ', ') AS siglas,
                            GROUP_CONCAT(sp.cnpj_sp separator ', ') AS cnpjs,
                            GROUP_CONCAT(sp.denominacao_sp separator '; ') AS denominacoes 
                        FROM sind_patr sp
                        INNER JOIN documento_sindicato_patronal_tb dspt ON dspt.documento_id = cg.doc_sind_id_documento
                        WHERE sp.id_sindp = dspt.sindicato_patronal_id
                    ) sindicatos_patronais ON true
                    LEFT JOIN LATERAL (
                        SELECT 
                            GROUP_CONCAT(sinde.sigla_sinde separator ', ') AS siglas,
                            GROUP_CONCAT(sinde.cnpj_sinde separator ', ') AS cnpjs,
                            GROUP_CONCAT(sinde.denominacao_sinde separator '; ') AS denominacoes 
                        FROM sind_emp sinde
                        INNER JOIN documento_sindicato_laboral_tb dslt ON dslt.documento_id = cg.doc_sind_id_documento
                        WHERE sinde.id_sinde = dslt.sindicato_laboral_id
                    ) sindicatos_laborais ON true
                    where ds.id_doc = cg.doc_sind_id_documento
                    and ec.resumivel = 1
                    GROUP by cg.doc_sind_id_documento, cg.estrutura_id_estruturaclausula, cg.texto_resumido
                    order by cg.doc_sind_id_documento, cg.estrutura_id_estruturaclausula");

                var result = await _context.SelectFromRawSqlAsync<RelatorioClausulasBuscaRapidaExcelViewModel>(query.ToString(), parameters);

                if (result is null)
                {
                    return NoContent();
                }

                var clausulasCliente = await (from cl in _context.ClausulaCliente
                                              join c in _context.ClausulaGerals on cl.ClausulaId equals c.Id
                                              where cl.GrupoEconomicoId == usuario.GrupoEconomicoId && request.ClausulasIds != null && request.ClausulasIds.Any(c => c == cl.ClausulaId)
                                              select new ClausulClienteRelatorioBuscaRapidaExcel
                                              {
                                                  Id = cl.Id,
                                                  GrupoEconomicoId = cl.GrupoEconomicoId,
                                                  ClausulaId = cl.ClausulaId,
                                                  DocumentoId = c.DocumentoSindicalId,
                                                  EstruturaClausulaId = c.EstruturaClausulaId,
                                                  Texto = cl.Texto
                                              })
                                              .ToListAsync();

                var documentos = RelatorioBuscaRapidaExcelConverter.Criar(result);

                var bytes = RelatorioBuscaRapidaExcelBuilder.Criar(documentos, clausulasCliente);

                if (bytes is null)
                {
                    return NoContent();
                }

                return DownloadExcel("realatorio_busca_rapida", bytes);
            }

            return NotAcceptable();
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> Criar([FromBody] IncluirClausulasGeraisRequest request)
        {
            var clausulaGeralRequest = new UpsertClausulaRequest
            {
                DocumentoSindicalId = request.DocumentoId,
                AssuntoId = request.AssuntoId,
                EstruturaClausulaId = request.EstruturaId,
                Numero = request.Numero,
                SinonimoId = request.SinonimoId,
                Texto = request.Texto,
                InformacoesAdicionais = request.InformacoesAdicionais
            };

            return await Dispatch(clausulaGeralRequest);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> Editar(int id, [FromBody] IncluirClausulasGeraisRequest request)
        {
            var clausulaGeralRequest = new UpsertClausulaRequest
            {
                Id = id,
                DocumentoSindicalId = request.DocumentoId,
                AssuntoId = request.AssuntoId,
                EstruturaClausulaId = request.EstruturaId,
                Numero = request.Numero,
                SinonimoId = request.SinonimoId,
                Texto = request.Texto,
                InformacoesAdicionais = request.InformacoesAdicionais
            };

            return await Dispatch(clausulaGeralRequest);
        }

        [HttpPut("aprovar/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> Aprovar([FromRoute] int id)
        {
            var request = new AprovarClausulaGeralRequest
            {
                Id = id
            };

            return await Dispatch(request);
        }

        [HttpPatch("{documentoId}/liberar")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> Liberar([FromRoute] int documentoId)
        {
            var request = new LiberarClausulasRequest
            {
                DocumentoId = documentoId
            };

            return await Dispatch(request);
        }

        [HttpPatch("{id}/texto-resumido")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> AtualizarResumo([FromBody] ClausulaGeralAtualizarResumoRequest request)
        {
            var atualizarTextoResumidoRequest = new AtualizarTextoResumidoRequest
            {
                Texto = request.Texto,
                EstruturaId = request.EstruturaId,
                DocumentoId = request.DocumentoId
            };

            return await Dispatch(atualizarTextoResumidoRequest);
        }


        [HttpPost("email-aprovado")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> EmailAprovado([FromBody] EnviarEmailClausulasAprovadasRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }

        [HttpPost("relatorios-clausulas")]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Octet)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> ObterRelatorioNegociacoes([FromBody] ClausulasGeraisRelatorioRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var relatorio = await _relatorioClausulaBuilder.HandleAsync(request.DocumentoId);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            var date = DateOnly.FromDateTime(DateTime.Now);

            var fileName = $"relatório_clausulas_ineditta_{date.Day}_{date.Month}_{date.Year}";

            return DownloadExcel(fileName, relatorio.Value);
        }

        [HttpGet("resumiveis/{documentoId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterResumiveis([FromRoute] int documentoId)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await _clausulaGeralRepository.ObterTodasPorEmpresaDocumentoId(documentoId);

            if (result is null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost("gerar-resumo/{documentoId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> GerarResumo([FromRoute] int documentoId)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var request = new ResumirClausulaRequest
            {
                DocumentoId = documentoId
            };

            return await Dispatch(request);
        }
    }
}
