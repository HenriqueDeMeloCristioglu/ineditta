using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.API.Constants;
using Microsoft.AspNetCore.OutputCaching;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using System.Text;
using MySqlConnector;
using Ineditta.Repository.Extensions;
using System.Globalization;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Application.Usuarios.Entities;
using Microsoft.Extensions.Options;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using TipoModulo = Ineditta.Application.Documentos.Sindicais.Entities.TipoModulo;
using Ineditta.BuildingBlocks.Core.FileStorage;
using Ineditta.Application.Documentos.Sindicais.UseCases.Aprovar;
using Microsoft.AspNetCore.Authorization;
using Ineditta.BuildingBlocks.Core.Web.API.Filters;
using Ineditta.BuildingBlocks.Core.Tokens;
using Ineditta.Application.SharedKernel.Frontend;
using Ineditta.Application.SharedKernel.GestaoDeChamados;
using Ineditta.Application.Documentos.Sindicais.UseCases.Liberar;
using Ineditta.API.ViewModels.Shared.Requests;
using Ineditta.API.ViewModels.DocumentosLocalizados.ViewModels;
using Ineditta.API.ViewModels.DocumentosSindicais.ViewModels;
using Ineditta.API.ViewModels.DocumentosSindicais.Requests;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.NegociacoesAcumuladas;
using Ineditta.API.ViewModels.SindicatosLaborais.Requests;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;
using Ineditta.API.Factories.Sindicatos;
using Ineditta.Application.ModulosEstaticos.Entities;
using Ineditta.Application.Documentos.Sindicais.UseCases.IniciarScrap;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.UseCases.AtualizarDataSla;
using Ineditta.Application.Documentos.Sindicais.UseCases.EnviarEmailAprovacao;
using Ineditta.Application.Documentos.Sindicais.UseCases.NotificarCriacao;
using Ineditta.Application.Documentos.Sindicais.UseCases.UploadFile;
using Ineditta.Application.Documentos.Sindicais.UseCases.UpsertComercial;
using Ineditta.Application.Documentos.Sindicais.UseCases.UpsertSisap;

namespace Ineditta.API.Controllers.V1.DocumentosSindicais
{
    [Route("v{version:apiVersion}/documentos-sindicais")]
    [ApiController]
    public class DocumentoSindicalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly HttpRequestItemService _httpRequestItemService;
        private readonly FileStorageConfiguration _fileStorageConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly FrontendConfiguration _frontendConfiguration;
        private readonly GestaoDeChamadoConfiguration _gestaoDeChamadoConfigurations;
        private readonly IMediator _mediator;
        private readonly SindicatoLaboralFactory _sindicatoLaboralFactory;
        private readonly SindicatoPatronalFactory _sindicatoPatronalFactory;

        private const int QUANTIDADE_DIAS_NEGOCIACOES = 60;

        public DocumentoSindicalController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, IOptions<FileStorageConfiguration> fileStorageConfiguration, HttpRequestItemService httpRequestItemService, IHttpClientFactory httpClientFactory, ITokenService tokenService, IOptions<FrontendConfiguration> frontendConfiguration, IOptions<GestaoDeChamadoConfiguration> gestaoDeChamadoConfiguration, SindicatoLaboralFactory sindicatoLaboralFactory, SindicatoPatronalFactory sindicatoPatronalFactory) : base(mediator, requestStateValidator)
        {
            _mediator = mediator;
            _context = context;
            _userInfoService = userInfoService;
            _httpRequestItemService = httpRequestItemService;
            _fileStorageConfiguration = fileStorageConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileStorageConfiguration));
            _httpRequestItemService = httpRequestItemService;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _frontendConfiguration = frontendConfiguration?.Value ?? throw new ArgumentNullException(nameof(frontendConfiguration));
            _gestaoDeChamadoConfigurations = gestaoDeChamadoConfiguration?.Value ?? throw new ArgumentNullException(nameof(gestaoDeChamadoConfiguration));
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentosSindicaisAprovadosViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentoSindicalDataTableProcessadosViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DocumentoSincicalRequestQuery request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                if (request.Processados is not null && request.Processados == true)
                {
                    var resultProcessados = await (from docSind in _context.DocSinds
                                                   join tipoDoc in _context.TipoDocs on docSind.TipoDocumentoId equals tipoDoc.Id into _tipoDoc
                                                   from tipoDoc in _tipoDoc.DefaultIfEmpty()
                                                   where tipoDoc.Processado
                                                   select new DocumentoSindicalSelectViewModel
                                                   {
                                                       Id = docSind.Id,
                                                       Description = tipoDoc.Nome,
                                                       ValidadeInicial = docSind.DataValidadeInicial,
                                                       ValidadeFinal = docSind.DataValidadeFinal,
                                                   })
                                        .ToListAsync();

                    return Ok(resultProcessados);
                }

                var moduloSisap = TipoModulo.SISAP;
                var result = await (from docSind in _context.DocSinds
                                    join tipoDoc in _context.TipoDocs on docSind.TipoDocumentoId equals tipoDoc.Id into _tipoDoc
                                    from tipoDoc in _tipoDoc.DefaultIfEmpty()
                                    where docSind.DataAprovacao != null && docSind.Modulo == moduloSisap
                                    select new DocumentoSindicalSelectViewModel
                                    {
                                        Id = docSind.Id,
                                        Description = tipoDoc.Nome,
                                        ValidadeInicial = docSind.DataValidadeInicial,
                                        ValidadeFinal = docSind.DataValidadeFinal,
                                    })
                                    .ToListAsync();
                return Ok(result);
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                if (request.Processados != null && request.Processados == true)
                {
                    var parameters = new List<MySqlParameter>();

                    StringBuilder queryBuilder = new(@"
                        SELECT * FROM documentos_sindicais_sisap_vw vw
                        WHERE TRUE
                    ");

                    if (request.Filter is not null)
                    {
                        queryBuilder.Append(@"
                            AND(
                                vw.Id LIKE CONCAT('%', @filter, '%')
                                OR vw.NomeDocumento LIKE CONCAT('%', @filter, '%')
                                OR JSON_EXTRACT(vw.CnaeDocs, '$[*]') LIKE CONCAT('%', @filter, '%')
                                OR DATE_FORMAT(vw.DataAprovacao, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                OR DATE_FORMAT(vw.ValidadeFinal, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                OR DATE_FORMAT(vw.DataSla, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                OR JSON_EXTRACT(vw.NomeSindicatoLaboral, '$[*]') LIKE CONCAT('%', @filter, '%')
                                OR JSON_EXTRACT(vw.NomeSindicatoPatronal, '$[*]') LIKE CONCAT('%', @filter, '%')
                                OR vw.NomeUsuarioAprovador LIKE CONCAT('%', @filter, '%')
                                OR DATE_FORMAT(vw.DataAssinatura, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                OR JSON_EXTRACT(vw.CnaeSubclasseCodigos, '$[*]') LIKE CONCAT('%', @filter, '%')
                            )
                         ");

                        parameters.Add(new MySqlParameter("@filter", request.Filter));
                    }

                    IQueryable<DocumentoSindicalDataTableProcessadosViewModel>? queryFilter = null;

                    queryFilter = _context.DocSindsSisap
                    .FromSqlRaw(queryBuilder.ToString(), parameters.ToArray())
                    .Select(ds => new DocumentoSindicalDataTableProcessadosViewModel
                    {
                        Id = ds.Id,
                        NomeDocumento = ds.NomeDoc,
                        CnaeDocs = ds.Cnaes,
                        DataAprovacao = ds.DataAprovacao,
                        ValidadeInicial = ds.DataValidadeInicial,
                        ValidadeFinal = ds.DataValidadeFinal,
                        NomeSindicatoLaboral = ds.SindicatosLaborais,
                        NomeSindicatoPatronal = ds.SindicatosPatronais,
                        NomeUsuarioAprovador = ds.UsuarioAprovador,
                        DataAssinatura = ds.DataAssinatura,
                        SubclasseCodigo = ds.CnaeSubclasseCodigos,
                        DataSla = ds.DataSla
                    });

                    request.Filter = string.Empty;

                    var result = await queryFilter.ToDataTableResponseAsync(request);

                    if (result == null)
                    {
                        return NoContent();
                    }

                    return Ok(result);
                }

                return Ok(await (from ds in _context.DocSinds
                                 join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                 from td in _td.DefaultIfEmpty()
                                 join dl in _context.DocumentosLocalizados on ds.DocumentoLocalizacaoId equals (int)dl.Id into _dl
                                 from dl in _dl.DefaultIfEmpty()
                                 join us in _context.UsuarioAdms on (long?)ds.UsuarioResponsavelId equals us.Id into _us
                                 from us in _us.DefaultIfEmpty()
                                 where ds.DataAprovacao != null && ds.DataAprovacao > new DateOnly(1900, 1, 1)
                                 select new DocumentosSindicaisAprovadosViewModel
                                 {
                                     Id = ds.Id,
                                     Nome = td.Nome,
                                     DataAprovacao = ds.DataAprovacao,
                                     DataSla = ds.DataSla,
                                     DataScrap = ds.DataScrap,
                                     NomeUsuarioResponsavel = us.Nome,
                                     Caminho = dl.CaminhoArquivo,
                                     Scrap = ds.ScrapAprovado
                                 })
                                .AsNoTracking()
                                .ToDataTableResponseAsync(request));
            }

            return Ok(await _context.DocSinds
                            .AsNoTracking()
                            .Select(dc => new OptionModel<int>
                            {
                                Id = dc.Id,
                                Description = dc.Database
                            }).ToListAsync());
        }

        [HttpGet("doc/{id}")]
        [Produces(MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentoSindicalViewModel>), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from ds in _context.DocSinds
                                where ds.Id == id
                                select new DocumentoSindicalViewModel
                                {
                                    Id = ds.Id,
                                    DataInicial = ds.DataValidadeInicial,
                                    DataFinal = ds.DataValidadeFinal,
                                    Abrangencias = ds.Abrangencias,
                                    DataLiberacaoClausulas = ds.DataLiberacao,
                                    Estabelecimentos = ds.Estabelecimentos
                                })
                                .SingleOrDefaultAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("anos-meses")]
        [OutputCache(PolicyName = ApiConstant.Policies.ExpireAfter5Minutes)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<DataBaseDocumentoViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json)]

        public async ValueTask<IActionResult> ObterAnosMesesAsync([FromQuery] DocumentoSindicalAnoMesRequest request)
        {
            if (request.PorUsuario is not null && request.PorUsuario.Value)
            {
                var parameters = new List<MySqlParameter>();
                int sqlParametersCount = 0;

                var query = new StringBuilder(@"SELECT DISTINCT ds.database_doc
                                                FROM doc_sind ds
                                                INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                                INNER JOIN tipo_doc tdt ON tdt.idtipo_doc = ds.tipo_doc_idtipo_doc
                                                WHERE CASE WHEN uat.nivel = @nivel THEN true 
                                                     WHEN uat.nivel = @nivelGrupoEconomico then 
                                                            JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                                    WHEN uat.nivel = @nivelMatriz THEN
                                                            JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""m"": ', CAST((
                                                                SELECT cut.cliente_matriz_id_empresa FROM cliente_unidades cut WHERE cut.id_unidade IN (
                                                                    SELECT jt.ids FROM JSON_TABLE(uat.ids_fmge, '$[*]' COLUMNS (ids INT PATH '$')) as jt
                                                                ) LIMIT 1
                                                            ) AS CHAR), '}}'))
                                                     else 
                                                JSON_CONTAINS(ds.cliente_estabelecimento , CONCAT('{{""u"": ', CAST((JSON_EXTRACT(uat.ids_fmge, '$[0]')) AS CHAR), '}}'))
                                                END");

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

                if (request.SindLaboralIds != null && request.SindLaboralIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindLaboralIds, "ds.sind_laboral", "id", parameters, ref sqlParametersCount);
                }

                if (request.SindPatronalIds != null && request.SindPatronalIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindPatronalIds, "ds.sind_patronal", "id", parameters, ref sqlParametersCount);
                }

                if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.GruposEconomicosIds, "ds.cliente_estabelecimento", "g", parameters, ref sqlParametersCount);
                }

                if (request.EmpresasIds != null && request.EmpresasIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.EmpresasIds, "ds.cliente_estabelecimento", "m", parameters, ref sqlParametersCount);
                }

                if (request.EstabelecimentosIds != null && request.EstabelecimentosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.EstabelecimentosIds, "ds.cliente_estabelecimento", "u", parameters, ref sqlParametersCount);
                }

                if (request.AtividadesEconomicasIds != null && request.AtividadesEconomicasIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.AtividadesEconomicasIds, "ds.cnae_doc", "id", parameters, ref sqlParametersCount);
                }

                if (request.LocalizacoesEstadosUfs != null && request.LocalizacoesEstadosUfs.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.LocalizacoesEstadosUfs, "ds.abrangencia", "Uf", parameters, ref sqlParametersCount);
                }

                if (request.LocalizacoesMunicipiosIds != null && request.LocalizacoesMunicipiosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.LocalizacoesMunicipiosIds, "ds.abrangencia", "id", parameters, ref sqlParametersCount);
                }

                if (request.NomeDocumentoIds != null && request.NomeDocumentoIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.NomeDocumentoIds, "ds.tipo_doc_idtipo_doc", parameters, ref sqlParametersCount);
                }

                if (request.Processados is not null)
                {
                    var processados = request.Processados.Value ? "S" : "N";

                    query.Append(" AND tdt.processado = @processados ");
                    parameters.Add(new MySqlParameter("@processados", processados));
                }

                var resultPorGrupoEconomico = await _context.DocSinds.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .Select(ds => new DataBaseDocumentoViewModel
                    {
                        MesAno = ds.Database
                    })
                    .ToListAsync();

                return resultPorGrupoEconomico is null || !resultPorGrupoEconomico.Any() ? NoContent() : Ok(resultPorGrupoEconomico);
            }

            var result = await _context.DocSinds
                    .Select(ds => new DataBaseDocumentoViewModel
                    {
                        MesAno = ds.Database
                    })
                    .Distinct()
                    .ToListAsync();

            return result is null || !result.Any() ? NoContent() : Ok(result);
        }

        [HttpPost("consultas")]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        [Produces(MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ConsultaDocumentoSindicalFormularioAplicacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ConsultaDocumentoSindicalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterConsultasSindicaisAsync([FromBody] ConsultaDocumentoSindicalRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            request ??= new ConsultaDocumentoSindicalRequest();

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return Unauthorized();
            }

            var sqlParameters = new List<MySqlParameter>();
            var sqlParametersCount = 0;

            var queryBuilder = new StringBuilder(@"
                        SELECT 
                            vw.*, 
                            abrangencia_usuario.value AS abrangencia_usuario, 
                            vw.sindicatos_laborais_siglas AS siglas_laborais, 
                            vw.sindicatos_patronais_siglas AS siglas_patronais,
                            vw.assuntos AS assuntos_string_for_search
                        FROM documento_sindical_vw vw                                             
                        INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                        LEFT JOIN LATERAL (
                            SELECT GROUP_CONCAT(DISTINCT CONCAT(l.municipio,'/',l.uf) SEPARATOR ', ') as value, GROUP_CONCAT(l.id_localizacao SEPARATOR ', ') as a
                            FROM cliente_unidades cut
                            JOIN LATERAL (
                            SELECT uni.id, vw.id_doc FROM 
	                            JSON_TABLE(vw.cliente_estabelecimento, '$[*]' COLUMNS (
	                             id INT PATH '$.u'
	                          )) uni
	                        ) unidades ON TRUE
                            INNER JOIN localizacao l ON l.id_localizacao = cut.localizacao_id_localizacao
                            WHERE CASE WHEN uat.nivel = 'Ineditta' THEN TRUE
                                       WHEN uat.nivel = 'Grupo Econômico' THEN uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                       ELSE JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade))
                                  END
                                  AND cut.id_unidade = unidades.id
                        ) as abrangencia_usuario ON TRUE
                        WHERE
                            CASE
                                WHEN uat.nivel = @nivelIneditta THEN true
                                WHEN uat.nivel = @nivelGrupoEconomico THEN
                                    -- Usuários Grupo Econômico podem visualizar qualquer documento realizado por membros de sua rede 
                                    (uat.id_grupoecon = vw.usuario_cadastro_grupo_economico and vw.usuario_cadastro_nivel <> @nivelIneditta)
                                    -- Usuários Grupo Econômico podem visualizar os documentos vinculados a membros de sua rede 
                                    OR JSON_CONTAINS(vw.cliente_estabelecimento , CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                    -- Podem ver também documentos cadastrados por usuários Ineditta que estejam sem vinculo
                                    OR (vw.usuario_cadastro_nivel = @nivelIneditta and (vw.cliente_estabelecimento is null or vw.cliente_estabelecimento = JSON_ARRAY()))
                                ELSE 
                                    -- Usuários Matriz/Empresa ou Unidade/Estabelecimento só podem visualizar os documentos vinculados a seus estabelecimentos
                                    EXISTS (
                                    	SELECT 1 FROM documento_estabelecimento_tb det 
                                    	WHERE det.documento_id = vw.id_doc
                                    		    AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(det.estabelecimento_id))
                                    )
                                    -- Documentos não vinculados realizados por membros de sua rede OU documentos não vinculados realizados por usuarios Ineditta
                                    OR ((vw.cliente_estabelecimento is null OR vw.cliente_estabelecimento = JSON_ARRAY()) AND (uat.id_grupoecon = vw.usuario_cadastro_grupo_economico OR vw.usuario_cadastro_nivel = @nivelIneditta))
                            END
                        and CASE WHEN uat.documento_restrito = 0 THEN vw.doc_restrito <> 'sim'
                                 ELSE true
                            END
                        and CASE WHEN vw.modulo = 'SISAP' THEN vw.data_inclusao IS NOT NULL 
                                 ELSE TRUE
                            END
                        AND vw.cliente_estabelecimento IS NOT NULL AND vw.cliente_estabelecimento <> JSON_ARRAY()
                        ");

            sqlParameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            sqlParameters.Add(new MySqlParameter("@nivelIneditta", nameof(Nivel.Ineditta)));
            sqlParameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
            sqlParameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

            queryBuilder.Append(request.TipoConsulta.ToLower(CultureInfo.InvariantCulture) == "geral" ? " and vw.modulo = 'COMERCIAL'" : " and vw.modulo = 'SISAP'");

            if (request.ClausulaAprovada != null && request.ClausulaAprovada == true)
            {
                queryBuilder.Append(@" AND EXISTS(select 1 from clausula_geral cg where cg.doc_sind_id_documento = vw.id_doc AND cg.aprovado = 'sim')");
            }

            if (request.Processados is not null && request.Processados == true)
            {
                queryBuilder.Append("AND vw.processado = 'S'");
            }

            if (request.Liberados is not null && request.Liberados == true)
            {
                queryBuilder.Append("AND vw.data_liberacao_clausulas > '1900-00-00'");
            }

            if (request.TiposDocumentos != null && request.TiposDocumentos.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.TiposDocumentos, "vw.tipo_documento", sqlParameters, ref sqlParametersCount);
            }

            if (request.NomesDocumentos != null && request.NomesDocumentos.Any())
            {
                var tipoDocIds = string.Join(", ", request.NomesDocumentos);

#pragma warning disable CA1305 // Specify IFormatProvider
                queryBuilder.Append($"AND vw.tipo_doc_id IN ({tipoDocIds})");
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            if (request.GrupoClausulaIds != null && request.GrupoClausulaIds.Any())
            {
                var queryGrupoClausulas = new StringBuilder(@" AND EXISTS(
                    SELECT 1 FROM estrutura_clausula ec
                    LEFT JOIN (
                        SELECT estrutura_id_estruturaclausula
                        FROM clausula_geral cg
                        WHERE cg.doc_sind_id_documento = vw.id_doc
                    ) AS estruturas ON ec.id_estruturaclausula = estruturas.estrutura_id_estruturaclausula
                    WHERE 1 = 1");

                QueryBuilder.AppendListToQueryBuilder(queryGrupoClausulas, request.GrupoClausulaIds, "ec.grupo_clausula_idgrupo_clausula", sqlParameters, ref sqlParametersCount);

                queryGrupoClausulas.Append(@"GROUP BY ec.grupo_clausula_idgrupo_clausula )");

                queryBuilder.Append(queryGrupoClausulas);
            }

            if (request.DataAprovacaoInicial.HasValue && request.DataAprovacaoFinal.HasValue)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                queryBuilder.Append(@" AND (vw.data_liberacao_clausulas >= @validadeLiberacaoInicial AND vw.data_liberacao_clausulas <= @validadeLiberacaoFinal)");

                sqlParameters.Add(new MySqlParameter("@validadeLiberacaoInicial", request.DataAprovacaoInicial.Value.ToString("yyyy-MM-dd")));
                sqlParameters.Add(new MySqlParameter("@validadeLiberacaoFinal", request.DataAprovacaoFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            if (request.TipoDocumentoId is not null && request.TipoDocumentoId > 0)
            {
                queryBuilder.Append(@" AND vw.tipo_doc_id = @tipoDocId");
                sqlParameters.Add(new MySqlParameter("@tipoDocId", request.TipoDocumentoId));
            }

            if (request.DataValidadeInicial.HasValue && request.UsarComparacaoEstritaNasValidades)
            {
                queryBuilder.Append(@" AND vw.validade_inicial = @dataInclusaoInicial");
                sqlParameters.Add(new MySqlParameter("@dataInclusaoInicial", request.DataValidadeInicial.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (request.DataValidadeFinal.HasValue && request.UsarComparacaoEstritaNasValidades)
            {
                queryBuilder.Append(@" AND vw.validade_final = @dataInclusaoFinal");
                sqlParameters.Add(new MySqlParameter("@dataInclusaoFinal", request.DataValidadeFinal.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (request.DataValidadeInicial.HasValue && request.DataValidadeFinal.HasValue && !request.UsarComparacaoEstritaNasValidades)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                queryBuilder.Append(@" AND (
                    (vw.data_inclusao BETWEEN @dataInclusaoInicial AND @dataInclusaoFinal) 
                )");

                sqlParameters.Add(new MySqlParameter("@dataInclusaoInicial", request.DataValidadeInicial.Value.ToString("yyyy-MM-dd")));
                sqlParameters.Add(new MySqlParameter("@dataInclusaoFinal", request.DataValidadeFinal.Value.ToString("yyyy-MM-dd")));
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            if (request.AnuenciaObrigatoria)
            {
                queryBuilder.Append(" AND vw.anuencia = 'Sim'");
            }

            if (request.Restrito)
            {
                queryBuilder.Append(" AND (vw.doc_restrito = 'Sim' and uat.documento_restrito = 1)");
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any() && usuario.Nivel != Nivel.GrupoEconomico && usuario.Nivel != Nivel.Matriz && usuario.Nivel != Nivel.Unidade)
            {
                queryBuilder.Append(@"
                    AND (
     	                TRUE 
                ");

                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.GruposEconomicosIds, "vw.cliente_estabelecimento", "g", sqlParameters, ref sqlParametersCount);

                queryBuilder.Append(@"
                        OR ((vw.cliente_estabelecimento is null OR vw.cliente_estabelecimento = JSON_ARRAY()) AND (uat.id_grupoecon = vw.usuario_cadastro_grupo_economico OR vw.usuario_cadastro_nivel = 'Ineditta'))
                    )
                ");
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any() && usuario.Nivel != Nivel.Matriz && usuario.Nivel != Nivel.Unidade)
            {
                queryBuilder.Append(@"
                    AND (
     	                TRUE 
                ");

                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.MatrizesIds, "vw.cliente_estabelecimento", "m", sqlParameters, ref sqlParametersCount);

                queryBuilder.Append(@"
                        OR ((vw.cliente_estabelecimento is null OR vw.cliente_estabelecimento = JSON_ARRAY()) AND (uat.id_grupoecon = vw.usuario_cadastro_grupo_economico OR vw.usuario_cadastro_nivel = 'Ineditta'))
                    )
                ");
            }

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                queryBuilder.Append(@"
                    AND (
     	                TRUE 
                ");

                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.UnidadesIds, "vw.cliente_estabelecimento", "u", sqlParameters, ref sqlParametersCount);

                queryBuilder.Append(@"
                        OR ((vw.cliente_estabelecimento is null OR vw.cliente_estabelecimento = JSON_ARRAY()) AND (uat.id_grupoecon = vw.usuario_cadastro_grupo_economico OR vw.usuario_cadastro_nivel = 'Ineditta'))
                    )
                ");
            }

            if (request.SindicatosLaboraisIds != null && request.SindicatosLaboraisIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosLaboraisIds, "vw.sind_laboral", "id", sqlParameters, ref sqlParametersCount);
            }

            if (request.SindicatosPatronaisIds != null && request.SindicatosPatronaisIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.SindicatosPatronaisIds, "vw.sind_patronal", "id", sqlParameters, ref sqlParametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                var cnaesIdsString = request.CnaesIds.Select(id => id.ToString(CultureInfo.InvariantCulture));
                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.CnaesIds, "vw.cnae_doc", "id", cnaesIdsString, "vw.cnae_doc", "id", sqlParameters, ref sqlParametersCount);
            }

            if (request.MunicipiosIds != null && request.MunicipiosIds.Any() || request.Ufs != null && request.Ufs.Any())
            {
                if (request.SindicatosLaboraisIds == null || !request.SindicatosLaboraisIds.Any())
                {
                    var sindicatoLaboralFactoryRequest = new SindicatoLaboralRequest
                    {
                        GruposEconomicosIds = request.GruposEconomicosIds,
                        MatrizesIds = request.MatrizesIds,
                        ClientesUnidadesIds = request.UnidadesIds,
                        CnaesIds = request.CnaesIds,
                        LocalizacoesIds = request.MunicipiosIds,
                        Ufs = request.Ufs,
                    };

                    var sindicatosLaborais = await _sindicatoLaboralFactory.CriarPorUsuario(sindicatoLaboralFactoryRequest);
                    if (sindicatosLaborais != null && sindicatosLaborais.Any())
                    {
                        IEnumerable<int> ids = sindicatosLaborais.Select(sl => sl.Id).ToList();
                        QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, ids, "vw.sind_laboral", "id", sqlParameters, ref sqlParametersCount);
                    }
                }

                if (request.SindicatosPatronaisIds == null || !request.SindicatosPatronaisIds.Any())
                {
                    var sindicatoPatronalFactoryRequest = new SindicatoPatronalRequest
                    {
                        GruposEconomicosIds = request.GruposEconomicosIds,
                        MatrizesIds = request.MatrizesIds,
                        ClientesUnidadesIds = request.UnidadesIds,
                        CnaesIds = request.CnaesIds,
                        LocalizacoesIds = request.MunicipiosIds,
                        Ufs = request.Ufs,
                    };

                    var sindicatosPatronais = await _sindicatoPatronalFactory.CriarPorUsuario(sindicatoPatronalFactoryRequest);
                    if (sindicatosPatronais != null && sindicatosPatronais.Any())
                    {
                        IEnumerable<int> ids = sindicatosPatronais.Select(sl => sl.Id).ToList();
                        QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, ids, "vw.sind_patronal", "id", sqlParameters, ref sqlParametersCount);
                    }
                }

                if (request.MunicipiosIds != null && request.MunicipiosIds.Any() && request.Ufs != null && request.Ufs.Any())
                {
                    var municipiosIdsString = request.MunicipiosIds.Select(id => id.ToString(CultureInfo.InvariantCulture));
                    queryBuilder.Append(@" AND ( ( TRUE ");
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.MunicipiosIds, "vw.abrangencia", "id", request.Ufs, "vw.abrangencia", "Uf", sqlParameters, ref sqlParametersCount);
                    queryBuilder.Append(@" ) OR ( TRUE ");
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, municipiosIdsString, "vw.abrangencia", "id", sqlParameters, ref sqlParametersCount);
                    queryBuilder.Append(@" ) )");
                }
                else if (request.MunicipiosIds != null && request.MunicipiosIds.Any())
                {
                    var municipiosIdsString = request.MunicipiosIds.Select(id => id.ToString(CultureInfo.InvariantCulture));
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.MunicipiosIds, "vw.abrangencia", "id", municipiosIdsString, "vw.abrangencia", "id", sqlParameters, ref sqlParametersCount);
                }
                else if (request.Ufs != null && request.Ufs.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.Ufs, "vw.abrangencia", "Uf", sqlParameters, ref sqlParametersCount);
                }
            }

            if (request.EstruturaClausulasIds != null && request.EstruturaClausulasIds.Any())
            {
                QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, request.EstruturaClausulasIds, "vw.estrutura_clausulas_ids", sqlParameters, ref sqlParametersCount);
            }

            if (request.AssuntosIds != null && request.AssuntosIds.Any())
            {
                var assuntosIdsString = request.AssuntosIds.Select(id => id.ToString(CultureInfo.InvariantCulture));
                queryBuilder.Append(@" AND ( ( TRUE ");
                QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, request.AssuntosIds, "vw.estrutura_clausulas_ids", sqlParameters, ref sqlParametersCount);
                queryBuilder.Append(@" ) OR ( TRUE ");
                QueryBuilder.AppendListArrayToQueryBuilder(queryBuilder, assuntosIdsString, "vw.estrutura_clausulas_ids", false, sqlParameters, ref sqlParametersCount);
                queryBuilder.Append(@" ) )");
            }

            if (request.DatasBases != null && request.DatasBases.Any(db => db.ToLowerInvariant() == "vigente"))
            {
                queryBuilder.Append(@" AND vw.validade_inicial <= @hoje AND (vw.validade_final >= @hoje OR vw.validade_final IS NULL)");

                var datasBases = request.DatasBases.ToList();
                datasBases.Remove("vigente");
                request.DatasBases = datasBases;

                sqlParameters.Add(new MySqlParameter("@hoje", DateTime.Now));
            }

            if (request.DatasBases != null && request.DatasBases.Any(db => db.ToLowerInvariant() == "ultimo ano"))
            {
                await _context.SelectFromRawSqlAsync<dynamic>(@"CALL obter_documento_ultimo_ano_sindicatos_laborais(@usuarioId); ", new Dictionary<string, object>() { { "@usuarioId", usuario.Id } });

                queryBuilder.Append(@"
                    AND vw.id_doc IN (
                        SELECT documento_sindical_id FROM documento_sindicato_mais_recente_usuario_tb dsmrut
                        WHERE dsmrut.usuario_id = @usuarioId
                    )
                ");

                var datasBases = request.DatasBases.ToList();
                datasBases.Remove("ultimo ano");
                request.DatasBases = datasBases;

                sqlParameters.Add(new MySqlParameter("@usuarioId", usuario.Id));
            }

            if (request.DatasBases != null && request.DatasBases.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.DatasBases, "vw.database_doc", sqlParameters, ref sqlParametersCount);
            }

            if (request.InformacoesAdicionais)
            {
                queryBuilder.Append(@" and exists (
	                select 1 from clausula_geral_estrutura_clausula cgec
	                where cgec.doc_sind_id_doc = vw.id_doc
                ) ");
            }

            if (request.ShortTemplate is not null && request.ShortTemplate == true)
            {
                var resultFormularioAplicacao = await _context.DocumentosSindicaisVw.FromSqlRaw(queryBuilder.ToString(), sqlParameters.ToArray())
                .AsNoTracking()
                .Select(dsvw => new ConsultaDocumentoSindicalFormularioAplicacaoViewModel
                {
                    Id = dsvw.Id,
                    Nome = dsvw.Nome,
                    AtividadesEconomicas = dsvw.AtividadesEconomicas,
                    Arquivo = dsvw.Arquivo,
                    SindicatosPatronais = dsvw.SindicatoPatronal,
                    SindicatosLaborais = dsvw.SindicatoLaboral,
                    DataLiberacao = dsvw.DataLiberacaoClausulas,
                    DataValidadeInicial = dsvw.DataValidadeInicial,
                    DataValidadeFinal = dsvw.DataValidadeFinal,
                })
                .ToDataTableResponseAsync(request);

                return Ok(resultFormularioAplicacao);
            }

            var result = await _context.DocumentosSindicaisVw.FromSqlRaw(queryBuilder.ToString(), sqlParameters.ToArray())
                .AsNoTracking()
                .Select(dsvw => new ConsultaDocumentoSindicalViewModel
                {
                    Arquivo = dsvw.Arquivo,
                    DataUpload = dsvw.DataUpload,
                    DataValidadeInicial = dsvw.DataValidadeInicial,
                    DataValidadeFinal = dsvw.DataValidadeFinal,
                    Descricao = dsvw.Descricao,
                    Comentarios = dsvw.Observacao,
                    Id = dsvw.Id,
                    Nome = dsvw.Nome,
                    SiglasSindicatosLaborais = dsvw.SindicatosLaboraisSiglas,
                    SiglasSindicatosPatronais = dsvw.SindicatosPatronaisSiglas,
                    SindicatosLaboraisIds = dsvw.SindicatosLaboraisIds,
                    SindicatosPatronaisIds = dsvw.SindicatosPatronaisIds,
                    AtividadesEconomicas = dsvw.AtividadesEconomicas,
                    DataInclusao = dsvw.DataInclusao,
                    Assuntos = dsvw.Assuntos,
                    AssuntosStringForSearch = dsvw.AssuntosStringForSearch,
                    Abrangencia = dsvw.Abrangencias,
                    MunicipiosEstabelecimentos = dsvw.AbrangenciaUsuario,
                    SiglasSindicatosLaboraisString = dsvw.SindicatosLaboraisSiglasString,
                    SiglasSindicatosPatronaisString = dsvw.SindicatosPatronaisSiglasString
                })
                .ToDataTableResponseAsync(request);

            return Ok(result);
        }

        [HttpGet("encerrados")]
        [Produces(MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentoSindicalNegociacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterEncerradosAsync([FromQuery] FiltroHomeDatatableRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var sql = new StringBuilder(@"select vw.* 
                                        FROM documento_sindical_vw vw  
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        WHERE case when uat.nivel = @nivel then true
                                        	when uat.nivel = @nivelGrupoEconomico 
	  	                                        then JSON_CONTAINS(cast(vw.cliente_estabelecimento as json), CONCAT('{{""g"":', cast(uat.id_grupoecon as char), '}}'))
                                            else
	                                        exists(
			                                        select 1 from 
			                                        json_table(cast(vw.cliente_estabelecimento as json), '$[*].u' columns (value int path '$') ) as jt1
			                                        join json_table(uat.ids_fmge, '$[*]' columns (value int path '$') ) as jt2
			                                        on jt1.value = jt2.value)
                                        end
                                        and vw.data_aprovacao IS NOT NULL 
                                        and vw.nome_doc IS NOT NULL 
                                        and vw.data_aprovacao >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)
                                        and vw.modulo = @modulo");

            var sqlParameters = new List<MySqlParameter>
            {
                new MySqlParameter("@email", _userInfoService.GetEmail()),
                new MySqlParameter("@nivel", nameof(Nivel.Ineditta)),
                new MySqlParameter("@dias", QUANTIDADE_DIAS_NEGOCIACOES),
                new MySqlParameter("@modulo", "SISAP"),
                new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription())
            };

            var sqlParametersCount = 0;

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.UnidadesIds, "vw.cliente_estabelecimento", "u", sqlParameters, ref sqlParametersCount);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.MatrizesIds, "vw.cliente_estabelecimento", "m", sqlParameters, ref sqlParametersCount);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.GruposEconomicosIds, "vw.cliente_estabelecimento", "g", sqlParameters, ref sqlParametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.CnaesIds, "vw.cnae_doc", "id", sqlParameters, ref sqlParametersCount);
            }

            if (!string.IsNullOrEmpty(request.Filter))
            {
                sql.AppendLine(@" and (
                                        JSON_CONTAINS(vw.sindicatos_laborais_municipios, JSON_QUOTE(@filter))
                                     OR JSON_CONTAINS(vw.sindicatos_laborais_siglas, JSON_QUOTE(@filter))
                                     OR JSON_CONTAINS(vw.sindicatos_patronais_siglas, JSON_QUOTE(@filter))
                                     OR JSON_EXTRACT(vw.sindicatos_laborais_municipios, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR JSON_EXTRACT(vw.sindicatos_laborais_siglas, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR JSON_EXTRACT(vw.sindicatos_patronais_siglas, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR DATE_FORMAT(vw.data_aprovacao, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                     OR database_doc like CONCAT('%', @filter, '%')
                                     OR vw.sigla_doc like CONCAT('%', @filter, '%')
                                      ) ");

                sqlParameters.Add(new MySqlParameter("@filter", request.Filter));

                request.Filter = string.Empty;
            }

            sql.AppendLine(@" order by vw.data_aprovacao desc");

            var result = await _context.DocumentosSindicaisVw
                                               .FromSqlRaw(sql.ToString(), sqlParameters.ToArray())
                                               .Select(dsvw => new DocumentoSindicalNegociacaoEncerradasViewModel
                                               {
                                                   Id = dsvw.Id,
                                                   SiglaDoc = dsvw.SiglaDoc,
                                                   Arquivo = dsvw.Arquivo,
                                                   SindicatosLaboraisSiglas = dsvw.SindicatosLaboraisSiglas,
                                                   SindicatosPatronaisSiglas = dsvw.SindicatosPatronaisSiglas,
                                                   SindicatosLaboraisMunicipios = dsvw.SindicatosLaboraisMunicipios,
                                                   DataBase = dsvw.DataBase,
                                                   DataAprovacao = dsvw.DataAprovacao
                                               })
                                               .ToDataTableResponseAsync(request);

            return Ok(result);
        }

        [HttpGet("processados")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentoSindicalNegociacaoViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterProcessadosAsync([FromQuery] FiltroHomeDatatableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {

                var sql = new StringBuilder(@"select vw.* 
                                        FROM documento_sindical_clausulas_vw vw  
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        WHERE case when uat.nivel = @nivel then true 
                                        when uat.nivel = @nivelGrupoEconomico 
	  	                                        then JSON_CONTAINS(cast(vw.cliente_estabelecimento as json), CONCAT('{{""g"":', cast(uat.id_grupoecon as char), '}}'))
                                        else exists(
			                                        select 1 from 
			                                        json_table(cast(vw.cliente_estabelecimento as json), '$[*].u' columns (value int path '$') ) as jt1
			                                        join json_table(uat.ids_fmge, '$[*]' columns (value int path '$') ) as jt2
			                                        on jt1.value = jt2.value)
                                        end
                                        and vw.clausula_quantidade_nao_aprovadas = 0
                                        and vw.nome_doc IS NOT NULL 
                                        and vw.clausula_data_ultima_aprovacao >= DATE_SUB(CURDATE(), INTERVAL @dias DAY)");


                var sqlParameters = new List<MySqlParameter>
            {
                new MySqlParameter("@email", _userInfoService.GetEmail()),
                new MySqlParameter("@nivel", nameof(Nivel.Ineditta)),
                new MySqlParameter("@dias", QUANTIDADE_DIAS_NEGOCIACOES),
                new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription())
            };

                var sqlParametersCount = 0;

                if (request.UnidadesIds != null && request.UnidadesIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, request.UnidadesIds, "vw.cliente_estabelecimento", "u", sqlParameters, ref sqlParametersCount);
                }

                if (request.MatrizesIds != null && request.MatrizesIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, request.MatrizesIds, "vw.cliente_estabelecimento", "m", sqlParameters, ref sqlParametersCount);
                }

                if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, request.GruposEconomicosIds, "vw.cliente_estabelecimento", "g", sqlParameters, ref sqlParametersCount);
                }

                if (request.CnaesIds != null && request.CnaesIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, request.CnaesIds, "vw.cnae_doc", "id", sqlParameters, ref sqlParametersCount);
                }

                if (!string.IsNullOrEmpty(request.Filter))
                {
                    sql.AppendLine(@" and (
                                        JSON_CONTAINS(vw.sindicatos_laborais_municipios, JSON_QUOTE(@filter))
                                     OR JSON_CONTAINS(vw.sindicatos_laborais_siglas, JSON_QUOTE(@filter))
                                     OR JSON_CONTAINS(vw.sindicatos_patronais_siglas, JSON_QUOTE(@filter))
                                     OR JSON_EXTRACT(vw.sindicatos_laborais_municipios, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR JSON_EXTRACT(vw.sindicatos_laborais_siglas, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR JSON_EXTRACT(vw.sindicatos_patronais_siglas, '$[*]') LIKE CONCAT('%', @filter, '%')
                                     OR DATE_FORMAT(vw.data_aprovacao, '%d/%m/%Y') like CONCAT('%', @filter, '%')
                                     OR database_doc like CONCAT('%', @filter, '%')
                                     OR vw.sigla_doc like CONCAT('%', @filter, '%')
                                      ) ");

                    sqlParameters.Add(new MySqlParameter("@filter", request.Filter));

                    request.Filter = string.Empty;
                }

                sql.AppendLine(@"order by vw.data_aprovacao desc");

                var result = await _context.DocumentoSindicalClausulasVw
                                                   .FromSqlRaw(sql.ToString(), sqlParameters.ToArray())
                                                   .Select(dsvw => new DocumentoSindicalNegociacaoViewModel
                                                   {
                                                       Id = dsvw.Id,
                                                       SiglaDoc = dsvw.SiglaDoc,
                                                       SindicatosLaboraisSiglas = dsvw.SindicatosLaboraisSiglas,
                                                       SindicatosPatronaisSiglas = dsvw.SindicatosPatronaisSiglas,
                                                       SindicatosLaboraisMunicipios = dsvw.SindicatosLaboraisMunicipios,
                                                       DataAprovacao = dsvw.ClausulaDataUltimaAprovacao,
                                                       DataBase = dsvw.DataBase,
                                                   })
                                                   .ToDataTableResponseAsync(request);

                return Ok(result);
            }

            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var resultSelects = (from ds in _context.DocSinds
                                           join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id
                                           where ds.Modulo == TipoModulo.SISAP
                                           select new OptionModel<int>
                                           {
                                               Id = td.Id,
                                               Description = td.Nome
                                           })
                                           .AsEnumerable()
                                           .DistinctBy(o => o.Description)
                                           .ToList();

                await Task.CompletedTask;

                if (resultSelects is null)
                {
                    return NoContent();
                }

                return Ok(resultSelects);
            }

            return NotAcceptable();
        }

        [HttpGet("negociacoes-acumuladas")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<NegociacaoAcumuladaViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterNegociacoesAcumuladasAsync([FromQuery] NegociacaoAcumuladaRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var sql = new StringBuilder($@"select IFNULL(year(dst.data_aprovacao), '0') as ano,
                                        count(tdt.idtipo_doc) as total,
                                        count(CASE WHEN tdt.nome_doc like 'Acordo Coletivo' THEN tdt.idtipo_doc ELSE NULL END) as acordo_coletivo,
                                        count(CASE WHEN tdt.nome_doc like 'Acordo Coletivo Específico' THEN tdt.idtipo_doc ELSE NULL END) as acordo_coletivo_especifico,
                                        count(CASE WHEN tdt.nome_doc like 'Convenção Coletiva' THEN tdt.idtipo_doc ELSE NULL END) as convencao_coletiva,
                                        count(CASE WHEN tdt.nome_doc like 'Convenção Coletiva Específica' THEN tdt.idtipo_doc ELSE NULL END) as convencao_coletiva_especifica,
                                        count(CASE WHEN tdt.nome_doc like 'Termo Aditivo de Convenção Coletiva' THEN tdt.idtipo_doc ELSE NULL END) as termo_aditivo_convencao_coletiva,
                                        count(CASE WHEN tdt.nome_doc like 'Termo Aditivo de Acordo Coletivo' THEN tdt.idtipo_doc ELSE NULL END) as termo_aditivo_acordo_coletivo,
                                        max(dst.data_aprovacao) maior_data_aprovacao
                                        from doc_sind dst 
                                        JOIN tipo_doc tdt ON tdt.idtipo_doc = dst.tipo_doc_idtipo_doc 
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        WHERE case when uat.nivel = @nivel then true 
                                                   when uat.nivel = @nivelGrupoEconomico
                                                   then JSON_CONTAINS(dst.cliente_estabelecimento, CONCAT('{{""g"":', cast(uat.id_grupoecon as char), '}}'))
                                        else 
	                                        exists(select 1 from 
	                                               json_table(dst.cliente_estabelecimento, '$[*].u' columns (value int path '$') ) as jt1
	                                               join json_table(uat.ids_fmge, '$[*]' columns (value int path '$') ) as jt2
	                                               on jt1.value = jt2.value) end
                                        and NOT isnull(dst.id_doc)
                                        and tdt.nome_doc in ('Acordo Coletivo', 'Acordo Coletivo Específico', 'Convenção Coletiva', 'Convenção Coletiva Específica', 'Termo Aditivo de Convenção Coletiva', 'Termo Aditivo de Acordo Coletivo')
                                        and year(dst.data_aprovacao) BETWEEN @anoInicial and @anoFinal");

            var sqlParameters = new Dictionary<string, object>()
            {
                { "@email", _userInfoService.GetEmail()! },
                { "@nivel", nameof(Nivel.Ineditta)},
                { "@anoInicial", request.AnoInicial },
                { "@anoFinal", request.AnoFinal },
                { "@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()! }
            };

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.UnidadesIds, "dst.cliente_estabelecimento", "u", sqlParameters);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.MatrizesIds, "dst.cliente_estabelecimento", "m", sqlParameters);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.GruposEconomicosIds, "dst.cliente_estabelecimento", "g", sqlParameters);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.CnaesIds, "dst.cnae_doc", "id", sqlParameters);
            }

            sql.Append(@" GROUP BY ANO");

            var result = await _context.SelectFromRawSqlAsync<NegociacaoAcumuladaTipoViewModel>(sql.ToString(), sqlParameters);

            return result == null || !result.Any() ? NoContent() : Ok(result.Select(natv => (NegociacaoAcumuladaViewModel)natv));
        }

        [HttpPost("documentos")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [SwaggerResponseMimeType(StatusCodes.Status404NotFound, typeof(Envelope), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> DonwloadDocumentoAsync([FromBody] DocumentoSindicalDownloadRequest request)
        {
            var documento = await (from ds in _context.DocSinds
                                   join dl in _context.DocumentosLocalizados on (long?)ds.DocumentoLocalizacaoId equals dl.Id into _dl
                                   from dl in _dl.DefaultIfEmpty()
                                   where ds.Id == request.Id
                                   select new
                                   {
                                       Caminho = ds.CaminhoArquivo,
                                       NomeDocumentoLocalizado = dl.NomeDocumento,
                                       NomeDocumentoComercial = ds.NomeArquivo
                                   }).FirstOrDefaultAsync();

            if (documento == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(documento.Caminho))
            {
                return BadRequest(Errors.Http.BadRequest("Informe o arquivo"));
            }

            if (Uri.TryCreate(documento.Caminho, UriKind.Absolute, out var uri))
            {
                if (documento.NomeDocumentoLocalizado is not null || documento.NomeDocumentoComercial is not null)
                {
                    return await DownloadFileFromUri(uri, documento.NomeDocumentoLocalizado ?? documento.NomeDocumentoComercial, async (uri) =>
                    {
                        using var httpClient = _httpClientFactory.CreateClient();

                        var response = await httpClient.GetAsync(uri);

                        return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                    });
                }

                return await DownloadFileFromUri(uri, async (uri) =>
                {
                    using var httpClient = _httpClientFactory.CreateClient();

                    var response = await httpClient.GetAsync(uri);

                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                });
            }

            var filePath = $"{_fileStorageConfiguration.Path}/documentos_sindicais/{documento.Caminho}";

            if (documento.NomeDocumentoLocalizado is not null || documento.NomeDocumentoComercial is not null)
            {
                return DownloadFile(filePath, documento.NomeDocumentoLocalizado ?? documento.NomeDocumentoComercial);
            }

            return DownloadFile(filePath);
        }

        [HttpGet("clientes-cnaes")]
        [Produces(SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> DocumentoPorClientesCanesAsync([FromQuery] DocumentoPorClienteCnaeRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var sqlParameters = new List<MySqlParameter>();
            var sqlParametersCount = 0;

            var queryBuilder = new StringBuilder(@"SELECT * from doc_sind ds
                        where exists (
	                        select * from documento_estabelecimento_tb det
	                        inner join cliente_unidades cu on det.estabelecimento_id = cu.id_unidade
	                        where det.documento_id = ds.id_doc
                        ");

            if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", sqlParameters, ref sqlParametersCount);
            }

            if (request.EmpresasIds is not null && request.EmpresasIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.EmpresasIds, "cu.cliente_matriz_id_empresa", sqlParameters, ref sqlParametersCount);
            }

            if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", sqlParameters, ref sqlParametersCount);
            }

            queryBuilder.Append(" ) ");

            if (request.AtividadesEconomicasIds != null && request.AtividadesEconomicasIds.Any())
            {
                var cnaesIdsString = request.AtividadesEconomicasIds.Select(id => id.ToString(CultureInfo.InvariantCulture));
                QueryBuilder.AppendListJsonToQueryBuilder(queryBuilder, request.AtividadesEconomicasIds, "ds.cnae_doc", "id", cnaesIdsString, "ds.cnae_doc", "id", sqlParameters, ref sqlParametersCount);
            }
            
            var result = await _context.DocSinds.FromSqlRaw(queryBuilder.ToString(), sqlParameters.ToArray())
            .AsNoTracking()
                .Select(ds => new OptionModel<int>
                {
                    Id = ds.Id,
                    Description = $"{ds.Id}"
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("files")]
        public async ValueTask<IActionResult> UploadFileAsync([FromForm] UploadFileDocumentoSindicalRequest request)
        {
            if (Request.Form.Files is null || !Request.Form.Files.Any())
            {
                return BadRequestFromErrorMessage("Você deve fornecer o arquivo");
            }

            if (Request.Form.Files.Count > 1)
            {
                return BadRequestFromErrorMessage("Você deve fornecer um único arquivo.");
            }

            request.Arquivo = await GetFiles();

            var result = await _mediator.Send(request);
            return FromResult(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DocumentosLocalizadoViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterLocalizadosPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from ds in _context.DocSinds
                                join tp in _context.TipoDocs on ds.TipoDocumentoId equals tp.Id
                                join dc in _context.DocumentosLocalizados on ds.DocumentoLocalizacaoId equals (int)dc.Id
                                where ds.Id == id
                                select new DocumentosLocalizadoViewModel
                                {
                                    Id = ds.Id,
                                    Nome = tp.Nome,
                                    IdTipoDoc = tp.Id,
                                    Sigla = tp.Sigla,
                                    Uf = ds.Uf,
                                    VersaoDocumento = ds.Versao,
                                    Origem = ds.Origem,
                                    NumeroSolicitacaoMR = ds.NumeroSolicitacao,
                                    NumRegMTE = ds.NumeroRegistroMTE,
                                    ValidadeInicial = ds.DataValidadeInicial,
                                    ValidadeFinal = ds.DataValidadeFinal,
                                    ProrrogacaoDoc = ds.DataProrrogacao,
                                    DataAssinatura = ds.DataAssinatura,
                                    DataRegMTE = ds.DataRegistroMTE,
                                    Permissao = ds.Permissao,
                                    Observacao = ds.Observacao,
                                    IdTipoNegocio = ds.TipoNegocioId,
                                    NomeDocumento = dc.NomeDocumento,
                                    Caminho = dc.CaminhoArquivo,
                                    DocRestrito = ds.Restrito,
                                    Database = ds.Database,
                                    DataLiberacaoClausulas = ds.DataLiberacao,
                                    UsuarioResponsavel = ds.UsuarioResponsavelId,
                                    ClienteEstabelecimento = ds.Estabelecimentos,
                                    Referencia = ds.Referencias,
                                    SindLaboral = (from dsl in _context.DocumentosSindicatosLaborais
                                                   join sinde in _context.SindEmps on dsl.SindicatoLaboralId equals sinde.Id
                                                   where dsl.DocumentoSindicalId == id
                                                   select new SindicatoLaboral
                                                   {
                                                       Id = sinde.Id,
                                                       Cnpj = sinde.Cnpj.Value,
                                                       Codigo = sinde.CodigoSindical.Valor,
                                                       Municipio = sinde.Municipio,
                                                       Sigla = sinde.Sigla,
                                                       Uf = sinde.Uf,
                                                       Denominacao = sinde.Denominacao
                                                   })
                                                   .AsSplitQuery()
                                                   .ToList(),
                                    SindPatronal = (from dsp in _context.DocumentosSindicatosPatronais
                                                    join sindp in _context.SindPatrs on dsp.SindicatoPatronalId equals sindp.Id
                                                    where dsp.DocumentoSindicalId == id
                                                    select new SindicatoPatronal
                                                    {
                                                        Id = sindp.Id,
                                                        Cnpj = sindp.Cnpj.Value,
                                                        Codigo = sindp.CodigoSindical.Valor,
                                                        Municipio = sindp.Municipio,
                                                        Sigla = sindp.Sigla,
                                                        Uf = sindp.Uf,
                                                        Denominacao = sindp.Denominacao
                                                    })
                                                    .AsSplitQuery()
                                                    .ToList(),
                                    CnaeDoc = ds.Cnaes,
                                    Abrangencia = ds.Abrangencias,
                                    IdDocumentoLocalizado = ds.DocumentoLocalizacaoId,
                                }).AsNoTracking()
                    .ToListAsync();

            if (result == null || !result.Any())
            {
                return NoContent();
            }

            if (result[0].ClienteEstabelecimento is not null && result[0].ClienteEstabelecimento!.Any())
            {
                result[0].ClienteUnidades = await _context.ClienteUnidades.AsTracking().Where(u => result[0].ClienteEstabelecimento != null && result[0].ClienteEstabelecimento!.Select(e => e.Id).Contains(u.Id)).ToListAsync();
            }

            return Ok(result);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<IActionResult> Criar([FromBody] UpsertSisapDocumentoSindicalRequest request)
        {
            var insertResult = await _mediator.Send(request);
            if (insertResult.IsFailure)
            {
                return FromResult(insertResult);
            }

            return CreatedResult(insertResult.Value);
        }

        [HttpPost]
        [Route("comercial")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async ValueTask<IActionResult> Criar([FromBody] UpsertComercialDocumentoSindicalRequest request)
        {
            var insertResult = await _mediator.Send(request);
            if (insertResult.IsFailure)
            {
                return FromResult(insertResult);
            }

            return CreatedResult(insertResult.Value);
        }

        [HttpPut]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async ValueTask<IActionResult> Update([FromBody] UpsertSisapDocumentoSindicalRequest request)
        {
            return await Dispatch(request);
        }


        [HttpGet("usuarios-notificacoes/{id}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(UsuarioNotificacaoPorDocumentoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterUsuariosNotificacaoAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>()
            {
                { "@id", id },
                { "@nivel", Nivel.Ineditta.ToString() },
                { "@modulo", Modulo.Comercial.Clausulas.Id }
            };

            StringBuilder query = new(@"SELECT DISTINCT  ua.email_usuario email, cg.nome_grupoeconomico nome_grupo_economico, uatmdt.id modulo_id, uatmdt.consultar consultar,
                            ua.notifica_email 
                            FROM usuario_adm ua
                            LEFT JOIN doc_sind ds ON true
                            LEFT JOIN cliente_grupo cg ON ua.id_grupoecon = cg.id_grupo_economico,
                            json_table(ua.modulos_comercial, '$[*]' COLUMNS (id INT PATH '$.id', consultar varchar(1) path '$.Consultar')) uatmdt,
                            json_table(ds.cliente_estabelecimento, '$[*]' COLUMNS (grupo_economico_id INT PATH '$.g')) dctget 
                            WHERE ds.id_doc = @id
                            AND ua.nivel <> @nivel
                            AND ua.notifica_email = 1
                            and uatmdt.id = @modulo
                            and uatmdt.consultar = '1'
                            and ds.origem not in ('cliente implantação', 'cliente nova negociação')
                            and dctget.grupo_economico_id = ua.id_grupoecon");

            var result = await _context.SelectFromRawSqlAsync<UsuarioNotificacaoPorDocumentoViewModel>(query.ToString(), parameters);

            if (result is null)
            {
                return NoContent();
            }

            return Ok(result);
        }


        [HttpGet("{id}/grupos-empresas")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(GruposEconomicosEmpresasPorIdRequest), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterGruposEmpresasPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>()
            {
                { "@id", id }
            };

            StringBuilder query = new(@"select
                                        cgt.nome_grupoeconomico nome_grupo_economico,
                                        cmt.nome_empresa empresa,
                                        dct.abrangencia,
                                        dct.sind_patronal sindicatos_patronais,
                                        dct.sind_laboral sindicatos_laborais
                                        from doc_sind dct
                                        inner join tipo_doc tdt on dct.tipo_doc_idtipo_doc = tdt.idtipo_doc,
                                        json_table(dct.cliente_estabelecimento,  '$[*]' columns (
                                        grupo_economico_id INT PATH '$.g',
                                        matriz_id int path '$.m'
                                               )) as gt
                                        join cliente_grupo cgt on gt.grupo_economico_id = cgt.id_grupo_economico 
                                        join cliente_matriz cmt on gt.matriz_id = cmt.id_empresa 
                                        WHERE id_doc = @id");

            var result = await _context.SelectFromRawSqlAsync<GruposEconomicosEmpresasPorIdRequest>(query.ToString(), parameters);

            if (result is null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPatch("{id}/aprovar")]
        public async ValueTask<IActionResult> AprovarDocumentoAsync([FromRoute] long id)
        {
            var request = new AprovarDocumentoSindicalRequest { Id = id };
            return await Dispatch(request);
        }

        [HttpPatch("{id}/data-sla")]
        public async ValueTask<IActionResult> AtualizarDataSlaAsync([FromRoute] long id, [FromBody] AtualizarDataSlaRequest request)
        {
            request ??= new AtualizarDataSlaRequest
            {
                DocSindId = id
            };

            request.DocSindId = id;

            return await Dispatch(request);
        }

        [HttpPost("notificacao/criacao")]
        public async ValueTask<IActionResult> EnviarNotificacaoDocumentoCriado([FromBody] NotificarCriacaoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpGet("{id}/arquivos")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationTokenFilter))]
        public async ValueTask<IActionResult> DownloadAsync([FromRoute] long id)
        {
            var documentoSindical = await _context.DocSinds.AsNoTracking().SingleOrDefaultAsync(dct => dct.Id == id);

            if (documentoSindical is null)
            {
                return NotFound(Errors.Http.NotFound());
            }


            if (Uri.TryCreate(documentoSindical.CaminhoArquivo, UriKind.Absolute, out var uri))
            {
                return await DownloadFileFromUri(uri, async (uri) =>
                {
                    using var httpClient = _httpClientFactory.CreateClient();

                    var response = await httpClient.GetAsync(uri);

                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                });
            }

            var filePath = $"{_fileStorageConfiguration.Path}/{DocumentoSindical.PastaDocumento}/{documentoSindical.CaminhoArquivo}";

            return DownloadFile(filePath);
        }

        [HttpGet("{id}/emails-configuracoes")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(EmailConfiguracaoDocumentoSindicalViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public ValueTask<IActionResult> ObterConfiguracoesEmailAsync([FromRoute] long id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return new ValueTask<IActionResult>(NotAcceptable());
            }

            var dataExpiracao = DateTimeOffset.Now.AddYears(1) - DateTimeOffset.Now;

            var token = _tokenService.Create(dataExpiracao);
            var uri = $"{_frontendConfiguration.Url}/api/v1/documentos-sindicais/{id}/arquivos?code={token}";
            var gestaoDeChamadosUrl = $"{_gestaoDeChamadoConfigurations.Url}";

            var result = new EmailConfiguracaoDocumentoSindicalViewModel
            {
                GestaoDeChamadosUrl = gestaoDeChamadosUrl,
                Uri = uri
            };

            return new ValueTask<IActionResult>(Ok(result));
        }


        [HttpPatch("{id}/liberar")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> LiberarAsync([FromRoute] long id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var request = new LiberarDocumentoSindicalRequest
            {
                Id = id
            };

            return await Dispatch(request);
        }

        [HttpPost("aprovados/emails")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> EnviarNotificacaoAprovacao(EnviarEmailAprovacaoRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPost("{id}/scrap")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IniciarScrap([FromRoute] int id)
        {
            var request = new IniciarScrapDocumentoSindicalRequest
            {
                DocumentoId = id
            };

            return await Dispatch(request);
        }
    }
}
