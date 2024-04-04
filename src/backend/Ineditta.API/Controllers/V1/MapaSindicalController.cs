using System.Net.Mime;
using System.Text;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;

using QueryBuilder = Ineditta.Repository.Extensions.QueryBuilder;
using Ineditta.API.ViewModels.MapasSindicais.Requests.GerarExcel;
using Ineditta.Application.Usuarios.Entities;
using System.Globalization;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Medias;
using Ineditta.API.Builders.MapasSindicais;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativo;
using CSharpFunctionalExtensions;
using Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativos;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.API.Factories.Clausulas;
using Ineditta.BuildingBlocks.Core.Database.Interceptors;
using Ineditta.API.ViewModels.Usuariosadm.ViewModels;
using Ineditta.API.ViewModels.MapasSindicais.ViewModels.Comparativos;
using Ineditta.API.ViewModels.MapasSindicais.ViewModels.GerarExcel;
using Ineditta.API.Factories.Sindicatos;
using Ineditta.API.ViewModels.SindicatosLaborais.Requests;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/mapas-sindicais")]
    [ApiController]
    public class MapaSindicalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ComparativoMapaSindicalBuilder _comparativoMapaSindicalBuilder;
        private readonly DocumentoSindicalClausulaFactory _documentoSindicalClausulaFactory;
        private readonly SindicatoLaboralFactory _sindicatoLaboralFactory;
        private readonly SindicatoPatronalFactory _sindicatoPatronalFactory;

        public MapaSindicalController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService,
            ComparativoMapaSindicalBuilder comparativoMapaSindicalBuilder, IUsuarioRepository usuarioRepository, DocumentoSindicalClausulaFactory documentoSindicalClausulaFactory, 
            SindicatoLaboralFactory sindicatoLaboralFactory, SindicatoPatronalFactory sindicatoPatronalFactory)
            : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
            _comparativoMapaSindicalBuilder = comparativoMapaSindicalBuilder;
            _usuarioRepository = usuarioRepository;
            _documentoSindicalClausulaFactory = documentoSindicalClausulaFactory;
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
        }

        [HttpGet]
        [Route("documentos")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentoSindicalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<DocumentoSindicalViewModel>), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterDocumentos([FromQuery] DocumentoReferenciaRequest request)
        {
            var sqlParameters = new List<MySqlParameter>();
            var sqlParametersCount = 0;


            var sql = new StringBuilder(@"select vw.* 
                                        from comparativo_mapa_sindical_principal_vw vw
                                        inner join usuario_adm uat on uat.email_usuario = @email
                                        where case when uat.nivel = @nivel then true
	  	                                           when uat.nivel = @nivelGrupoEconomico then 
                                                    vw.estabelecimentos is not null and vw.estabelecimentos <> '[]' and 
                                                    exists(select 1 from json_table(cast(vw.estabelecimentos as json), '$[*].g' columns (value int path '$')) as jt1
           				                                   where jt1.value = uat.id_grupoecon)
	  	                                           else 
                                                    vw.estabelecimentos is not null and vw.estabelecimentos <> '[]' and 
                                                    exists(select 1 from 
	                                                           json_table(cast(vw.estabelecimentos as json), '$[*].u' columns (value int path '$') ) as jt1
	                                                           join json_table(uat.ids_fmge, '$[*]' columns (value int path '$') ) as jt2
	                                                           on jt1.value = jt2.value) end
                                        and vw.data_aprovacao is not null 
                                        and vw.quantidade_clausulas_comparativo > 0 ");

            sqlParameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            sqlParameters.Add(new MySqlParameter("@nivel", Nivel.Ineditta.ToString()));
            sqlParameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.AtividadesEconomicasIds != null && request.AtividadesEconomicasIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.AtividadesEconomicasIds, "vw.cnaes", "id", sqlParameters, ref sqlParametersCount);
            }

            if (request.DatasBases != null && request.DatasBases.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.DatasBases, "vw.data_base", sqlParameters, ref sqlParametersCount);
            }

            if (request.LocalizacoesEstadosUfs != null && request.LocalizacoesEstadosUfs.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.LocalizacoesEstadosUfs, "vw.abrangencia", "Uf", sqlParameters, ref sqlParametersCount);
            }

            if (request.LocalizacoesMunicipiosIds != null && request.LocalizacoesMunicipiosIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(sql, request.LocalizacoesMunicipiosIds, "vw.abrangencia", "id", sqlParameters, ref sqlParametersCount);
            }

            if ((request.LocalizacoesEstadosUfs != null && request.LocalizacoesEstadosUfs.Any()) || 
                (request.LocalizacoesMunicipiosIds != null && request.LocalizacoesMunicipiosIds.Any()))
            {
                var sindicatoLaboralFactoryRequest = new SindicatoLaboralRequest
                {
                    CnaesIds = request.AtividadesEconomicasIds,
                    LocalizacoesIds = request.LocalizacoesMunicipiosIds,
                    Ufs = request.LocalizacoesEstadosUfs,
                };

                var sindicatosLaborais = await _sindicatoLaboralFactory.CriarPorUsuario(sindicatoLaboralFactoryRequest);
                if (sindicatosLaborais != null && sindicatosLaborais.Any())
                {
                    IEnumerable<int> ids = sindicatosLaborais.Select(sl => sl.Id).ToList();
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, ids, "vw.sindicatos_laborais", "id", sqlParameters, ref sqlParametersCount);
                }

                var sindicatoPatronalFactoryRequest = new SindicatoPatronalRequest
                {
                    CnaesIds = request.AtividadesEconomicasIds,
                    LocalizacoesIds = request.LocalizacoesMunicipiosIds,
                    Ufs = request.LocalizacoesEstadosUfs,
                };

                var sindicatosPatronais = await _sindicatoPatronalFactory.CriarPorUsuario(sindicatoPatronalFactoryRequest);
                if (sindicatosPatronais != null && sindicatosPatronais.Any())
                {
                    IEnumerable<int> ids = sindicatosPatronais.Select(sl => sl.Id).ToList();
                    QueryBuilder.AppendListJsonToQueryBuilder(sql, ids, "vw.sindicatos_patronais", "id", sqlParameters, ref sqlParametersCount);
                }
            }

            if (request.NomeDocumentoIds != null && request.NomeDocumentoIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.NomeDocumentoIds, "vw.tipo_documento_id", sqlParameters, ref sqlParametersCount);
            }
            
            if (request.IgnorarDocumentoId is not null && request.IgnorarDocumentoId.Value > 0)
            {
                sql.Append(" and vw.documento_id != @ignorarDocumentoId");
                sqlParameters.Add(new MySqlParameter("@ignorarDocumentoId", request.IgnorarDocumentoId.Value));
            }

            var result = await _context.ComparativoMapaSindicalPrincipalVw.FromSqlRaw(sql.ToString(), sqlParameters.ToArray())
                .TagWith(InterceptorTag.SqlCalcFoundRows)
                .AsNoTracking()
                .Select(dsvw => new DocumentoSindicalViewModel
                {
                    DataUpload = dsvw.DataUpload,
                    DataValidadeInicial = dsvw.ValidadeInicial,
                    DataValidadeFinal = dsvw.ValidadeFinal,
                    Descricao = dsvw.DocumentoNome,
                    Id = dsvw.DocumentoId,
                    Nome = dsvw.DocumentoNome,
                    SindicatosLaborais = dsvw.SindicatosLaborais,
                    SindicatosPatronais = dsvw.SindicatosPatronais,
                    AtividadesEconomicas = dsvw.Cnaes,
                    DataBase = dsvw.Database,
                    CnaesSubclasses = dsvw.CnaesSubclasses,
                    SiglasSindicatosLaborais = dsvw.SiglasSindicatosLaborais,
                    SiglasSindicatosPatronais = dsvw.SiglasSindicatosPatronais
                })
                .ToDataTableV2ResponseAsync(_context, request);


            return Ok(result);
        }

        [HttpPost("excel")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<UsuariosAdmViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromBody] GerarExelRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"select vw.* from documento_mapa_sindical_vw vw
		                            INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                    WHERE vw.quantidade_clausulas = vw.quantidade_clausulas_liberadas                                      
                                    AND vw.quantidade_clausulas > 0
                                    AND  case when uat.nivel = @nivel then true 
                                             when uat.nivel = @nivelGrupoEconomico then 
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                             else exists(
			                                        select 1 from 
			                                        json_table(cast(vw.documento_estabelecimento as json), '$[*].u' columns (value int path '$') ) as jt1
			                                        join json_table(uat.ids_fmge, '$[*]' columns (value int path '$') ) as jt2
			                                        on jt1.value = jt2.value)
                                        end");

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                if (request.GrupoEconomicoIds != null && request.GrupoEconomicoIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.GrupoEconomicoIds, "vw.documento_estabelecimento", "g", parameters, ref parametersCount);
                }

                if (request.EmpresaIds != null && request.EmpresaIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.EmpresaIds, "vw.documento_estabelecimento", "m", parameters, ref parametersCount);
                }

                if (request.UnidadeIds != null && request.UnidadeIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.UnidadeIds, "vw.documento_estabelecimento", "u", parameters, ref parametersCount);
                }

                if (request.TipoDocIds != null && request.TipoDocIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.TipoDocIds, "vw.documento_tipo_id", parameters, ref parametersCount);
                }

                if (request.CnaeIds != null && request.CnaeIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.CnaeIds, "vw.documento_cnae", "id", parameters, ref parametersCount);
                }

                if (request.SindLaboralIds != null && request.SindLaboralIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindLaboralIds, "vw.documento_sindicato_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindPatronalIds != null && request.SindPatronalIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindPatronalIds, "vw.documento_sindicato_patronal", "id", parameters, ref parametersCount);
                }

                if (request.DataBases != null && request.DataBases.Any(db => db.ToLowerInvariant() == "vigente"))
                {
                    query.Append(@" AND vw.documento_validade_inicial <= @hoje AND (vw.documento_validade_final >= @hoje OR vw.documento_validade_final IS NULL)");

                    var datasBases = request.DataBases.ToList();
                    datasBases.Remove("vigente");
                    request.DataBases = datasBases;

                    parameters.Add(new MySqlParameter("@hoje", DateTime.Now));
                }

                if (request.DataBases != null && request.DataBases.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.DataBases, "vw.documento_database", parameters, ref parametersCount);
                }

                if (request.DocumentoIds != null && request.DocumentoIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.DocumentoIds, "vw.documento_id", parameters, ref parametersCount);
                }

                if (request.ProcessamentoInedittaInicial != null && request.ProcessamentoInedittaFinal != null)
                {
                    query.Append(" AND vw.data_liberacao_clausulas >= @dataLiberacaoInicial and vw.data_liberacao_clausulas <= @dataLiberacaoFinal");
                    parameters.Add(new MySqlParameter("@dataLiberacaoInicial", request.ProcessamentoInedittaInicial.Value.LocalDateTime.Date));
                    parameters.Add(new MySqlParameter("@dataLiberacaoFinal", request.ProcessamentoInedittaFinal.Value.LocalDateTime.Date));
                }

                if (request.LocalidadeIds != null && request.LocalidadeIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.LocalidadeIds, "vw.documento_abrangencia", "id", parameters, ref parametersCount);
                }

                if (request.Ufs != null && request.Ufs.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.Ufs, "vw.documento_abrangencia", "Uf", parameters, ref parametersCount);
                }

                if (request.LocalidadeIds != null && request.LocalidadeIds.Any() || request.Ufs != null && request.Ufs.Any())
                {
                    if (request.SindLaboralIds == null || !request.SindLaboralIds.Any())
                    {
                        var sindicatoLaboralFactoryRequest = new SindicatoLaboralRequest
                        {
                            GruposEconomicosIds = request.GrupoEconomicoIds,
                            MatrizesIds = request.EmpresaIds,
                            ClientesUnidadesIds = request.UnidadeIds,
                            CnaesIds = request.CnaeIds,
                            LocalizacoesIds = request.LocalidadeIds,
                            Ufs = request.Ufs,
                        };

                        var sindicatosLaborais = await _sindicatoLaboralFactory.CriarPorUsuario(sindicatoLaboralFactoryRequest);
                        if (sindicatosLaborais != null && sindicatosLaborais.Any())
                        {
                            IEnumerable<int> ids = sindicatosLaborais.Select(sl => sl.Id).ToList();
                            QueryBuilder.AppendListJsonToQueryBuilder(query, ids, "vw.documento_sindicato_laboral", "id", parameters, ref parametersCount);
                        }
                    }

                    if (request.SindPatronalIds == null || !request.SindPatronalIds.Any())
                    {
                        var sindicatoPatronalFactoryRequest = new SindicatoPatronalRequest
                        {
                            GruposEconomicosIds = request.GrupoEconomicoIds,
                            MatrizesIds = request.EmpresaIds,
                            ClientesUnidadesIds = request.UnidadeIds,
                            CnaesIds = request.CnaeIds,
                            LocalizacoesIds = request.LocalidadeIds,
                            Ufs = request.Ufs,
                        };

                        var sindicatosPatronais = await _sindicatoPatronalFactory.CriarPorUsuario(sindicatoPatronalFactoryRequest);
                        if (sindicatosPatronais != null && sindicatosPatronais.Any())
                        {
                            IEnumerable<int> ids = sindicatosPatronais.Select(sl => sl.Id).ToList();
                            QueryBuilder.AppendListJsonToQueryBuilder(query, ids, "vw.documento_sindicato_patronal", "id", parameters, ref parametersCount);
                        }
                    }
                }

                if ((request.GrupoClausulaIds != null && request.GrupoClausulaIds.Any()) ||
                    (request.EstruturaClausulaIds != null && request.EstruturaClausulaIds.Any()) ||
                    !string.IsNullOrEmpty(request.PalavraChave))
                {
                    var queryGrupoClausula = new StringBuilder(@" AND (select max(1) from clausula_geral cgt
                                                                            inner join estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                                                            inner join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
                                                                                where cgt.doc_sind_id_documento = vw.documento_id 
                                                                                  and cgt.liberado = 'S' ");

                    if ((request.GrupoClausulaIds?.Any() ?? false))
                    {
                        QueryBuilder.AppendListToQueryBuilder(queryGrupoClausula, request.GrupoClausulaIds, "gct.idgrupo_clausula", parameters, ref parametersCount);
                    }

                    if ((request.EstruturaClausulaIds?.Any() ?? false))
                    {
                        QueryBuilder.AppendListToQueryBuilder(queryGrupoClausula, request.EstruturaClausulaIds, "ect.id_estruturaclausula", parameters, ref parametersCount);
                    }

                    if (!string.IsNullOrEmpty(request.PalavraChave))
                    {
                        queryGrupoClausula.Append(" AND LOWER(cgt.tex_clau) like @palavraChave");
                        parameters.Add(new MySqlParameter("@palavraChave", "%" + request.PalavraChave.ToLower(CultureInfo.InvariantCulture) + "%"));
                    }

                    queryGrupoClausula.Append(@" AND (select max(1) from clausula_geral_estrutura_clausula cgec 
                                                                where cgec.clausula_geral_id_clau = cgt.id_clau
                                                                and cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170) > 0 ");

                    queryGrupoClausula.Append(" ) > 0 ");

                    query.Append(queryGrupoClausula);
                }

                var result = await _context.DocumentoMapaSindicalVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(dc => new GerarExcelViewModel
                    {
                        Id = dc.DocumentoId,
                        SindLaboral = dc.DocumentoSindicatoLaboral,
                        SindPatronal = dc.DocumentoSindicatoPatronal,
                        DataBase = dc.DocumentoDatabase,
                        ValidadeFinal = dc.DocumentoValidadeFinal,
                        DataLiberacao = dc.DataLiberacaoClausulas,
                        Aprovacao = dc.DocumentoDataAprovacao,
                        CnaeDoc = dc.DocumentoCnae,
                        UfDoc = dc.DocumentoUf,
                        TituloDoc = dc.TipoDocumentoNome,
                        Abrangencias = dc.DocumentoAbrangencia
                    }).ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }

        [HttpPost("excel/informacoes-adicionais")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> GerarExcelInformacoesAdicionaisAsync([FromBody] InformacaoAdicionalRequest request)
        {
            if (Request.Headers.Accept != MediaTypes.Excel)
            {
                return NotAcceptable();
            }

            if (request is null)
            {
                return EmptyRequestBody();
            }

            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var sql = new StringBuilder(@"
                SELECT 
                    cgiav.*,
                    unidades.codigos codigos_unidades,
	                unidades.cnpjs cnpjs_unidades,
	                unidades.codigos_sindicato_cliente codigos_sindicato_cliente_unidades,
                    unidades.ufs ufs_unidades,
                    unidades.municipios municipios_unidades
                FROM clausula_geral_info_adicional_vw cgiav 
                inner join lateral (
                    SELECT 
                        GROUP_CONCAT(DISTINCT cut.codigo_unidade separator ', ') AS codigos, 
                        GROUP_CONCAT(DISTINCT cut.cnpj_unidade separator ', ') AS cnpjs,
                        GROUP_CONCAT(DISTINCT cut.cod_sindcliente separator ', ') AS codigos_sindicato_cliente,
                        GROUP_CONCAT(DISTINCT l.uf separator ', ') AS ufs,
                        GROUP_CONCAT(DISTINCT l.municipio separator ', ') AS municipios
                    FROM cliente_unidades cut
                    INNER JOIN usuario_adm ua on ua.email_usuario = @email
                    INNER JOIN doc_sind dsindt2 on cgiav.documento_sindical_id = dsindt2.id_doc
                    LEFT JOIN localizacao l on l.id_localizacao = cut.localizacao_id_localizacao
                    LEFT JOIN LATERAL (
    	                SELECT det.estabelecimento_id AS id FROM documento_estabelecimento_tb det 
    	                WHERE det.documento_id = dsindt2.id_doc
                    ) estabelecimento ON TRUE
                    WHERE estabelecimento.id = cut.id_unidade AND
                          CASE WHEN ua.nivel = 'Ineditta' THEN TRUE
                               WHEN ua.nivel = 'Grupo Econômico' THEN cut.cliente_grupo_id_grupo_economico = ua.id_grupoecon
                               ELSE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
                          END
                ) unidades
                WHERE 1 = 1 
            ");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));

            QueryBuilder.AppendListToQueryBuilder(sql, request.DocumentosIds, "cgiav.documento_sindical_id", parameters, ref parametersCount);

            if (request.GrupoClausulaIds != null && request.GrupoClausulaIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.GrupoClausulaIds, "cgiav.grupo_clausula_id", parameters, ref parametersCount);
            }

            if (request.EstruturaClausulaIds != null && request.EstruturaClausulaIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.EstruturaClausulaIds, "cgiav.estrutura_clausula_id", parameters, ref parametersCount);
            }

            if (!string.IsNullOrEmpty(request.PalavraChave))
            {
                sql.Append(" AND LOWER(cgiav.clausula_texto) like @palavraChave");
                parameters.Add(new MySqlParameter("@palavraChave", "%" + request.PalavraChave.ToLower(CultureInfo.InvariantCulture) + "%"));
            }

            var clausulaQuery = await _context.ClausulaGeralInformacaoAdicionalVw
                                    .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                    .ToListAsync();

            if (clausulaQuery == null || !clausulaQuery.Any())
            {
                return NoContent();
            }     

            var bytes = MapaSindicalBuilder.Converter(clausulaQuery);

            return bytes == null ? NoContent() : DownloadExcel("mapa-sindical", bytes);
        }

        [HttpPost]
        [Route("excel/comparativo")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Octet)]
        public async ValueTask<IActionResult> DonwloadComparativoMapaSindicalExcelAsync([FromBody] GerarComparativoMapaSindicalExcelRequest request)
        {
            if (request.DocumentoReferenciaId <= 0)
            {
                return BadRequest(Errors.Http.BadRequest("Documento para referência não informado"));
            }

            if (request.DocumentoComparacaoIds == null)
            {
                return BadRequest(Errors.Http.BadRequest("Negociação não informada para comparativo"));
            }

            var documentos = new List<int>
            {
                request.DocumentoReferenciaId
            };

            documentos.AddRange(request.DocumentoComparacaoIds);

            var dados = _context.ComparativoMapaSindicalPrincipalVw.Where(x => documentos.Contains(x.DocumentoId)).ToList();

            if (!(dados?.Any() ?? false))
            {
                return NoContent();
            }

            var clausulas = await _context.ComparativoMapaSindicalItemVw.Where(cgiav => documentos.Contains(cgiav.DocumentoSindicalId) && cgiav.ClausulaInformacaoNumero > 0 && cgiav.ExibeComparativoMapaSindical).ToListAsync();
            
            if (!(clausulas?.Any() ?? false))
            {

                return NoContent();
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(_userInfoService.GetEmail()!);

            if (usuario is null)
            {
                return NoContent();
            }

            var result = _comparativoMapaSindicalBuilder.Converter(usuario, dados, clausulas);

            if (result.IsFailure)
            {
                return FromResult(result);
            }

            if (result.Value.Length == 0)
            {
                return FromResult(Result.Failure("Não tem dados processados para a seleção efetuada"));
            }

            return DownloadExcelAsync("comparativo_mapa_sindical", result.Value);
        }

        [HttpGet("formularios-aplicacoes")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterFormularioAplicacaoAsync([FromQuery] int documentoSindicalId)
        {
            var documentoSindicalClausulas = await _documentoSindicalClausulaFactory.CriarAsync(documentoSindicalId);

            if (documentoSindicalClausulas is null)
            {
                return NotFound(Errors.Http.NotFound());
            }

            await _context.DocumentoSindicatoEstabelecimentoVw.AsNoTracking().Where(p => p.DocumentoId == documentoSindicalId).ToListAsync();

            return NoContent();
        }
    }
}
