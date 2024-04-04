using System.Globalization;
using System.Net.Mime;
using System.Text;

using DinkToPdf;
using DinkToPdf.Contracts;
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

using DiffPlex;
using DiffPlex.DiffBuilder;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MySqlConnector;
using Newtonsoft.Json;
using Ineditta.API.Services;
using Ineditta.API.Filters;
using Ineditta.API.ViewModels.Clausulas.ViewModels;
using Ineditta.API.ViewModels.Clausulas.Requests;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.API.Factories.Sindicatos;
using Ineditta.API.ViewModels.SindicatosLaborais.Requests;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;
using Ineditta.API.Factories.Clausulas;
using Ineditta.Application.Comentarios.Entities;

namespace Ineditta.API.Controllers.V1.ClausulasControllers
{
    [Route("v{version:apiVersion}/clausulas")]
    [ApiController]
    public class ClausulaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly SindicatoLaboralFactory _sindicatoLaboralFactory;
        private readonly SindicatoPatronalFactory _sindicatoPatronalFactory;
        private readonly IUserInfoService _userInfoService;
        private readonly IConverter _converter;
        private readonly HttpRequestItemService _httpRequestItemService;

        public ClausulaController(IMediator mediator, SindicatoLaboralFactory sindicatoLaboralFactory, SindicatoPatronalFactory sindicatoPatronalFactory, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, IConverter converter, HttpRequestItemService httpRequestItemService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
            _converter = converter;
            _httpRequestItemService = httpRequestItemService;
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
        }

        [HttpPost]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ClausulaViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterTodosAsync([FromBody] ClausulaRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var usuario = _httpRequestItemService.ObterUsuario();

                if (usuario is null)
                {
                    return NotAcceptable();
                }

                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"select * from (select vw.* from clausulas_vw vw
                                    INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                    WHERE 
                                        vw.clausula_geral_liberada = 'S' and (vw.aprovado = 'Sim' or vw.aprovado = 'sim') and
                                        case when uat.nivel = @nivel then true 
                                             when uat.nivel = @nivelGrupoEconomico then 
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                            ELSE
                                                EXISTS (
				   	                                SELECT 1 FROM documento_estabelecimento_tb det 
				   	                                WHERE det.documento_id = vw.documento_id
				      	                                  AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(det.estabelecimento_id))
				                                )
                                    end) x
                                    where true
                                    ");

                if (request.TipoDataBase == "ultimo-ano")
                {
                    query = new StringBuilder(@"select * from (
                                    select vw.* from clausulas_vw vw
                                    INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                    INNER JOIN documento_sindicato_mais_recente_usuario_tb dsmrut on dsmrut.clausula_geral_id = vw.clausula_id and uat.id_user = dsmrut.usuario_id
                                    WHERE 
                                        vw.clausula_geral_liberada = 'S' and (vw.aprovado = 'Sim' or vw.aprovado = 'sim') and
                                        case when uat.nivel = @nivel then true 
                                             when uat.nivel = @nivelGrupoEconomico then 
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                            ELSE
                                                EXISTS (
				   	                                SELECT 1 FROM documento_estabelecimento_tb det 
				   	                                WHERE det.documento_id = vw.documento_id
				      	                                  AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(det.estabelecimento_id))
				                                )
                                    end) as x
                                    where true
                                    ");
                }

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

                if (request.IdDoc > 0)
                {
                    query.Append(" AND x.documento_id = @idDoc");
                    parameters.Add(new MySqlParameter("@idDoc", request.IdDoc));
                }

                if (request.GrupoEconomicoIds != null && request.GrupoEconomicoIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.GrupoEconomicoIds, "x.documento_estabelecimento", "g", parameters, ref parametersCount);
                }

                if (request.EmpresaIds != null && request.EmpresaIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.EmpresaIds, "x.documento_estabelecimento", "m", parameters, ref parametersCount);
                }

                if (request.UnidadeIds != null && request.UnidadeIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.UnidadeIds, "x.documento_estabelecimento", "u", parameters, ref parametersCount);
                }

                if (request.TipoDocIds != null && request.TipoDocIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.TipoDocIds, "x.documento_tipo_id", parameters, ref parametersCount);
                }

                if (request.CnaeIds != null && request.CnaeIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.CnaeIds, "x.documento_cnae", "id", parameters, ref parametersCount);
                }

                if (request.SindLaboralIds != null && request.SindLaboralIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindLaboralIds, "x.documento_sindicato_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindPatronalIds != null && request.SindPatronalIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindPatronalIds, "x.documento_sindicato_patronal", "id", parameters, ref parametersCount);
                }

                if (request.GrupoClausulaIds != null && request.GrupoClausulaIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.GrupoClausulaIds, "x.grupo_clausula_id", parameters, ref parametersCount);
                }

                if (request.EstruturaClausulaIds != null && request.EstruturaClausulaIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.EstruturaClausulaIds, "x.estrutura_clausula_id", parameters, ref parametersCount);
                }

                if (request.DocumentoIds != null && request.DocumentoIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.DocumentoIds, "x.documento_id", parameters, ref parametersCount);
                }

                if (request.MunicipiosIds != null && request.MunicipiosIds.Any() || request.Ufs != null && request.Ufs.Any())
                {
                    if (request.SindLaboralIds == null || !request.SindLaboralIds.Any())
                    {
                        var sindicatoLaboralFactoryRequest = new SindicatoLaboralRequest
                        {
                            GruposEconomicosIds = request.GrupoEconomicoIds,
                            MatrizesIds = request.EmpresaIds,
                            ClientesUnidadesIds = request.UnidadeIds,
                            CnaesIds = request.CnaeIds,
                            LocalizacoesIds = request.MunicipiosIds,
                            Ufs = request.Ufs,
                        };

                        var sindicatosLaborais = await _sindicatoLaboralFactory.CriarPorUsuario(sindicatoLaboralFactoryRequest);
                        if (sindicatosLaborais != null && sindicatosLaborais.Any())
                        {
                            IEnumerable<int> ids = sindicatosLaborais.Select(sl => sl.Id).ToList();
                            QueryBuilder.AppendListJsonToQueryBuilder(query, ids, "x.documento_sindicato_laboral", "id", parameters, ref parametersCount);
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
                            LocalizacoesIds = request.MunicipiosIds,
                            Ufs = request.Ufs,
                        };

                        var sindicatosPatronais = await _sindicatoPatronalFactory.CriarPorUsuario(sindicatoPatronalFactoryRequest);
                        if (sindicatosPatronais != null && sindicatosPatronais.Any())
                        {
                            IEnumerable<int> ids = sindicatosPatronais.Select(sl => sl.Id).ToList();
                            QueryBuilder.AppendListJsonToQueryBuilder(query, ids, "x.documento_sindicato_patronal", "id", parameters, ref parametersCount);
                        }
                    }

                    if (request.MunicipiosIds != null && request.MunicipiosIds.Any() && request.Ufs != null && request.Ufs.Any())
                    {
                        QueryBuilder.AppendListJsonToQueryBuilder(query, request.MunicipiosIds, "x.documento_abrangencia", "id", request.Ufs, "x.documento_abrangencia", "Uf", parameters, ref parametersCount);
                    }
                    else if (request.MunicipiosIds != null && request.MunicipiosIds.Any())
                    {
                        QueryBuilder.AppendListJsonToQueryBuilder(query, request.MunicipiosIds, "x.documento_abrangencia", "id", parameters, ref parametersCount);
                    }
                    else if (request.Ufs != null && request.Ufs.Any())
                    {
                        QueryBuilder.AppendListJsonToQueryBuilder(query, request.Ufs, "x.documento_abrangencia", "Uf", parameters, ref parametersCount);
                    }
                }
                
                if (!string.IsNullOrEmpty(request.PalavraChave))
                {
                    query.Append(" AND LOWER(x.clausula_texto) like @palavraChave collate utf8mb4_general_ci");
                    parameters.Add(new MySqlParameter("@palavraChave", "%" + request.PalavraChave.ToLower(CultureInfo.InvariantCulture) + "%"));
                }

                if (request.DataProcessamentoInicial is not null && request.DataProcessamentoFinal is not null)
                {
                    parameters.Add(new MySqlParameter("@dataProcessamentoInicial", request.DataProcessamentoInicial.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                    parameters.Add(new MySqlParameter("@dataProcessamentoFinal", request.DataProcessamentoFinal.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));

                    query.Append(@" AND x.data_processamento_documento >= @dataProcessamentoInicial AND x.data_processamento_documento <= @dataProcessamentoFinal ");
                }

                if (!request.NaoConstaNoDocumento)
                {
                    query.Append(" AND x.consta_no_documento = 1 ");
                }

                if (request.DataProcessamentoDocumento)
                {
                    query.Append(" AND x.data_processamento_documento is not null ");
                }

                if (request.Resumivel)
                {
                    query.Append(" AND x.resumivel = 1 ");
                }

                if (request.ClausulaGrupoEconomico && usuario.Nivel != Nivel.Ineditta)
                {
                    query.Append(@" and exists (
	                    SELECT 1 from estrutura_clausula_grupo_economico_tb ec2
	                    where ec2.grupo_economico_id = @grupoEconomicoId
	                    and ec2.estrutura_clausula_id = x.estrutura_clausula_id
                    ) ");

                    parameters.Add(new MySqlParameter("@grupoEconomicoId", usuario.GrupoEconomicoId));
                }

                if (request.TipoDataBase == "data-base")
                {
                    if (!string.IsNullOrEmpty(request.DataBase))
                    {
                        query.Append(" AND x.documento_database = @dataBase");
                        parameters.Add(new MySqlParameter("@dataBase", request.DataBase));
                    }

                    var result = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(cl => new ClausulaViewModel
                    {
                        Id = cl.ClausulaId,
                        NomeDocumento = cl.DocumentoNome,
                        GrupoClausula = cl.GrupoClausulaNome,
                        NomeClausula = cl.EstruturaClausulaNome,
                        SindLaboral = cl.DocumentoSindicatoLaboral,
                        SindPatronal = cl.DocumentoSindicatoPatronal,
                        TextoClausula = cl.ClausulaTexto,
                        DataBase = cl.DocumentoDatabase,
                        ValidadeFinal = cl.DocumentoValidadeFinal,
                        DataProcessamentoDocumento = cl.DataProcessamentoDocumento,
                        TextoResumido = cl.TextoResumido
                    }).ToDataTableResponseAsync(request);

                    return Ok(result);
                }

                if (request.TipoDataBase == "vigente")
                {
                    query.Append(" AND x.documento_validade_inicial <= @dataHoje and x.documento_validade_final >= @dataHoje");
                    parameters.Add(new MySqlParameter("@dataHoje", DateTime.Now.ToLocalTime().Date));

                    var result = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(cl => new ClausulaViewModel
                    {
                        Id = cl.ClausulaId,
                        NomeDocumento = cl.DocumentoNome,
                        GrupoClausula = cl.GrupoClausulaNome,
                        NomeClausula = cl.EstruturaClausulaNome,
                        SindLaboral = cl.DocumentoSindicatoLaboral,
                        SindPatronal = cl.DocumentoSindicatoPatronal,
                        TextoClausula = cl.ClausulaTexto,
                        DataBase = cl.DocumentoDatabase,
                        ValidadeFinal = cl.DocumentoValidadeFinal,
                        DataProcessamentoDocumento = cl.DataProcessamentoDocumento,
                        TextoResumido = cl.TextoResumido
                    }).ToDataTableResponseAsync(request);

                    return Ok(result);
                }

                if (request.TipoDataBase == "ultimo-ano")
                {
                    await _context.GetUltimoDocumentoClausulaDoSindicatoAsync(usuario.Id);

                    var result = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(cl => new ClausulaViewModel
                    {
                        Id = cl.ClausulaId,
                        NomeDocumento = cl.DocumentoNome,
                        GrupoClausula = cl.GrupoClausulaNome,
                        NomeClausula = cl.EstruturaClausulaNome,
                        SindLaboral = cl.DocumentoSindicatoLaboral,
                        SindPatronal = cl.DocumentoSindicatoPatronal,
                        TextoClausula = cl.ClausulaTexto,
                        DataBase = cl.DocumentoDatabase,
                        ValidadeFinal = cl.DocumentoValidadeFinal,
                        DataProcessamentoDocumento = cl.DataProcessamentoDocumento,
                        TextoResumido = cl.TextoResumido
                    }).ToDataTableResponseAsync(request);

                    return Ok(result);
                }

                return NoContent();
            }

            return NoContent();
        }

        [HttpPost]
        [Route("por-id")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulaViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromBody] ClausulaPorIdRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NoContent();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario is null)
            {
                return NotAcceptable();
            }

            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var query = new StringBuilder(@"select vw.* from clausulas_vw vw
                                    INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                    WHERE 
                                        vw.clausula_geral_liberada = 'S' and
                                        case when uat.nivel = @nivel then true 
                                             when uat.nivel = @nivelGrupoEconomico then 
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                             ELSE
                                                EXISTS (
				   	                                SELECT 1 FROM documento_estabelecimento_tb det 
				   	                                WHERE det.documento_id = vw.documento_id
				      	                                  AND JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(det.estabelecimento_id))
				                                )
                                    end
                                    ");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
            parameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

            if (request.ClausulaIds != null && request.ClausulaIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.ClausulaIds, "vw.clausula_id", parameters, ref parametersCount);
            }

            var result = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                .AsNoTracking()
                .Join(_context.DocSinds, cl => cl.DocumentoId, ds => ds.Id, (cl, ds) => new
                {
                    cl.ClausulaId,
                    DocumentoId = ds.Id,
                    cl.GrupoClausulaNome,
                    cl.EstruturaClausulaNome,
                    cl.DocumentoSindicatoLaboral,
                    cl.DocumentoSindicatoPatronal,
                    cl.ClausulaTexto,
                    cl.DocumentoDatabase,
                    cl.DocumentoValidadeInicial,
                    cl.DocumentoValidadeFinal,
                    cl.DataAprovacaoDocumento,
                    cl.DataAprovacaoClausula,
                    cl.DocumentoCnae,
                    cl.DocumentoEstabelecimento,
                    cl.DocumentoReferencia,
                    cl.DocumentoNome,
                    cl.TextoResumido,
                    ds.DataAssinatura
                })
                .Select(cl => new ClausulaViewModel
                {
                    Id = cl.ClausulaId,
                    GrupoClausula = cl.GrupoClausulaNome,
                    NomeClausula = cl.EstruturaClausulaNome,
                    SindLaboral = cl.DocumentoSindicatoLaboral,
                    SindPatronal = cl.DocumentoSindicatoPatronal,
                    TextoClausula = cl.ClausulaTexto,
                    DataBase = cl.DocumentoDatabase,
                    ValidadeInicial = cl.DocumentoValidadeInicial,
                    ValidadeFinal = cl.DocumentoValidadeFinal,
                    DataAprovacao = cl.DataAprovacaoDocumento,
                    DataAprovacaoClausula = cl.DataAprovacaoClausula,
                    Cnae = cl.DocumentoCnae,
                    Unidade = cl.DocumentoEstabelecimento,
                    Referencia = cl.DocumentoReferencia,
                    NomeDocumento = cl.DocumentoNome,
                    PossuiInformacaoAdicional = _context.InformacaoAdicionalSisap
                                                    .Where(cgec => cgec.ClausulaGeralId == cl.ClausulaId)
                                                    .AsSplitQuery()
                                                    .AsEnumerable()
                                                    .Any(),
                    TextoResumido = cl.TextoResumido,
                    DataAssinaturaDocumento = cl.DataAssinatura,
                    TextoResumidoCliente = _context.ClausulaCliente
                                    .Where(c => c.ClausulaId == cl.ClausulaId && c.GrupoEconomicoId == usuario.GrupoEconomicoId)
                                    .AsSplitQuery()
                                    .OrderByDescending(c => EF.Property<DateTime>(c, "DataInclusao"))
                                    .Select(c => c.Texto)
                                    .SingleOrDefault()
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("comentarios-por-id")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulaViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterComentariosPorIdAsync([FromBody] ClausulaPorIdRequest request)
        {
            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                var usuario = _httpRequestItemService.ObterUsuario();

                if (usuario == null) return NoContent();

#pragma warning disable IDE0075 // Simplify conditional expression
#pragma warning disable S1125 // Boolean literals should not be redundant
                var result = await (from cm in _context.Comentarios
                                    join ua in _context.UsuarioAdms on EF.Property<int>(cm, "UsuarioInclusaoId") equals ua.Id into _ua
                                    from ua in _ua.DefaultIfEmpty()
                                    join eq in _context.Etiquetas on cm.EtiquetaId equals eq.Id into _eq
                                    from eq in _eq.DefaultIfEmpty()
                                    where request.ClausulaIds.Any() && cm.Tipo == TipoComentario.Clausula && request.ClausulaIds.Contains(cm.ReferenciaId) && (usuario.Id != ua.Id ? cm.Visivel : true)
                                    select new ComentarioClausulaViewModel
                                    {
                                        IdClausula = cm.ReferenciaId,
                                        NomeUsuario = ua.Nome,
                                        DataRegistro = EF.Property<DateTime>(cm, "DataInclusao"),
                                        Comentario = cm.Valor,
                                        Etiqueta = eq.Nome,
                                        GrupoEconomicoId = ua.GrupoEconomicoId,
                                        EstabelecimentosIds = ua.EstabelecimentosIds
                                    })
                                    .ToListAsync();
#pragma warning restore S1125 // Boolean literals should not be redundant
#pragma warning restore IDE0075 // Simplify conditional expression

                if (usuario.Nivel != Nivel.Ineditta)
                {
#pragma warning disable S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
                    return Ok(result.Where(x => usuario.Nivel == Nivel.GrupoEconomico ? x.GrupoEconomicoId == usuario.GrupoEconomicoId : usuario.EstabelecimentosIds!.Any(id => x.EstabelecimentosIds!.Contains(id))));
#pragma warning restore S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
                }

                return Ok(result);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("datas-bases")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterDatasBasesAsync([FromQuery] ClausulaDatasBasesRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"select distinct vw.documento_database, SUBSTR(vw.documento_database, 5, 4) ano, SUBSTR(vw.documento_database, 1, 3) mes from clausulas_vw vw
                                    INNER JOIN usuario_adm uat ON uat.email_usuario = @email
                                    WHERE 
                                        vw.clausula_geral_liberada = 'S' and
                                        case when uat.nivel = @nivel then true 
                                             when uat.nivel = @nivelGrupoEconomico then 
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""g"": ', CAST(uat.id_grupoecon AS CHAR), '}}'))
                                            WHEN uat.nivel = @nivelMatriz THEN
                                                    JSON_CONTAINS(vw.documento_estabelecimento, CONCAT('{{""m"": ', CAST((
                                                        SELECT cut.cliente_matriz_id_empresa FROM cliente_unidades cut WHERE cut.id_unidade IN (
                                                            SELECT jt.ids FROM JSON_TABLE(uat.ids_fmge, '$[*]' COLUMNS (ids INT PATH '$')) as jt
                                                        ) LIMIT 1
                                                    ) AS CHAR), '}}'))
                                             else 
                                        JSON_CONTAINS(vw.documento_estabelecimento , CONCAT('{{""u"": ', CAST((JSON_EXTRACT(uat.ids_fmge, '$[0]')) AS CHAR), '}}'))
                                    end
                                    ");

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@nivelMatriz", Nivel.Matriz.GetDescription()));

                if (request.SindLaboralIds != null && request.SindLaboralIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindLaboralIds, "vw.documento_sindicato_laboral", "id", parameters, ref parametersCount);
                }

                if (request.SindPatronalIds != null && request.SindPatronalIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.SindPatronalIds, "vw.documento_sindicato_patronal", "id", parameters, ref parametersCount);
                }

                if (request.GrupoEconomicoIds != null && request.GrupoEconomicoIds.Any())
                {
                    QueryBuilder.AppendListJsonToQueryBuilder(query, request.GrupoEconomicoIds, "vw.documento_estabelecimento", "g", parameters, ref parametersCount);
                }

                query.Append(@" ORDER BY ano desc, FIELD(SUBSTR(vw.documento_database, 1, 3), 'JAN', 'FEV', 'MAR', 'ABR', 'MAI', 'JUN', 'JUL', 'AGO', 'SET', 'OUT', 'NOV', 'DEZ') DESC");

                var result = await _context.ClausulasVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(cl => new OptionModel<string>
                    {
                        Id = cl.DocumentoDatabase,
                        Description = cl.DocumentoDatabase,
                    }).ToListAsync();

                return Ok(result);
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("vigencia-select")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterVigenciaAsync([FromQuery] VigenciaSelectRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();

                var query = new StringBuilder(@"SELECT
		                        substr(doc.database_doc, 5) as ano,
		                        DATE_FORMAT(doc.validade_inicial, '%d/%m/%Y') inicial,
		                        DATE_FORMAT(doc.validade_final, '%d/%m/%Y') final
		                        FROM doc_sind as doc
                                        LEFT JOIN clausula_geral cgt on doc.id_doc = cgt.doc_sind_id_documento 
	                                    LEFT JOIN tipo_doc as tp on tp.idtipo_doc = doc.tipo_doc_idtipo_doc
	                                    WHERE tp.idtipo_doc = @tipoDocumentoId and (cgt.aprovado = 'Sim' or cgt.aprovado = 'sim') and cgt.liberado = 'S'
                                        and JSON_CONTAINS(doc.sind_laboral, JSON_OBJECT('id', @sindeId))
                                    ");

                if (request.SindeId > 0)
                {
                    parameters.Add(new MySqlParameter("@sindeId", request.SindeId));
                }

                if (request.SindpId > 0 && request.NomeDocumentoId != 5 && request.NomeDocumentoId != 47 && request.NomeDocumentoId != 13 && request.NomeDocumentoId != 8)
                {
                    query.Append(@" and JSON_CONTAINS(doc.sind_patronal, JSON_OBJECT('id', @sindpId))");
                    parameters.Add(new MySqlParameter("@sindpId", request.SindpId));
                }

                if (request.ValidadeInicialVigenciaReferencia.HasValue)
                {
                    query.Append(" AND doc.validade_inicial <= @vigenciaReferencia");
                    parameters.Add(new MySqlParameter("@vigenciaReferencia", request.ValidadeInicialVigenciaReferencia.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                }

                parameters.Add(new MySqlParameter("@tipoDocumentoId", request.NomeDocumentoId));

                query.Append(" GROUP BY database_doc ORDER BY substr(doc.database_doc, 5) DESC");

                Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();

                foreach (MySqlParameter parameter in parameters)
                {
                    if (parameter != null && parameter.Value != null)
                    {
                        parameterDictionary[parameter.ParameterName] = parameter.Value;
                    }
                }

                var result = await _context.SelectFromRawSqlAsync<dynamic>(query.ToString(), parameterDictionary);

                if (result != null)
                {
                    return Ok(result
                        .Select(x => new OptionModel<string>
                        {
                            Id = $"{x.ano} ({x.inicial}-{x.final})",
                            Description = $"{x.ano} ({x.inicial} - {x.final})",
                        }));
                }

                return NoContent();
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("nome-doc-select")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterNomesDocumentosAsync([FromQuery] NomeDocumentoSelectRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new Dictionary<string, object>();

                var query = new StringBuilder(@"select tp.nome_doc nome, tp.idtipo_doc id from doc_sind doc
                                                left join tipo_doc tp on doc.tipo_doc_idtipo_doc = tp.idtipo_doc
                                                where doc.modulo = 'SISAP' 
                                                      AND JSON_CONTAINS(doc.sind_laboral, JSON_OBJECT('id', @sindeId))
                                                ");

                if (request.SindeId > 0)
                {
                    parameters.Add("sindeId", request.SindeId);
                }

                if (request.SindpId > 0)
                {
                    query.Append(@" and JSON_CONTAINS(doc.sind_patronal, JSON_OBJECT('id', @sindpId))");
                    parameters.Add("sindpId", request.SindpId);
                }

                query.Append(@" group by tp.idtipo_doc");

                var result = await _context.SelectFromRawSqlAsync<dynamic>(query.ToString(), parameters);

                if (result != null)
                {
                    return Ok(result
                        .Select(x => new OptionModel<int>
                        {
                            Id = x.id,
                            Description = x.nome,
                        }));
                }

                return NoContent();
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("comparacao")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulaViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterComparacaoClausulasAsync([FromQuery] ComparacaoClausulaRequest request)
        {
            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                var sindPatronalReferencia = await _context.SelectFromRawSqlAsync<dynamic>(@"
                    SELECT ds.sind_patronal patronal FROM doc_sind ds
                    WHERE ds.id_doc = @idDocumentoReferencia
                ", new Dictionary<string, object>() { { "idDocumentoReferencia", request.DocumentoReferenciaId } });

                if (request.DocumentoReferenciaId > 0 && request.DocumentoAnteriorId > 0)
                {
                    var docs = sindPatronalReferencia.Select(x => new DocsClausulasViewModel
                    {
                        DocAnterior = request.DocumentoAnteriorId,
                        DocReferencia = request.DocumentoReferenciaId,
                        SindPatronal = x.patronal
                    }).SingleOrDefault();

                    var queryTextos = new StringBuilder(@"select * from (
                                                            select cgt.id_clau novo_clausula_id, 
                                                                   x.id_clau antigo_clausula_id,
                                                                   cgt.tex_clau novo_texto,
                                                                   x.tex_clau velho_texto,
                                                                   cgt.assunto_idassunto novo_assunto_id,
                                                                   x.assunto_idassunto antigo_assunto_id,
                                                                   ect.nome_clausula novo_nome_clausula,
                                                                   x.nome_clausula antigo_nome_clausula,
                                                                   gct.nome_grupo novo_nome_grupo_clausula,
                                                                   x.nome_grupo antigo_nome_grupo_clausula,
                                                                   cgt.numero_clausula novo_numero_clausula,
                                                                   x.numero_clausula antigo_numero_clausula,
                                                                   cgt.doc_sind_id_documento novo_documento_sindical_id,
                                                                  x.doc_sind_id_documento antigo_documento_sindical_id,
                                                                  cgt.estrutura_id_estruturaclausula novo_estutura_clausula_id,
                                                                  x.estrutura_id_estruturaclausula antigo_estutura_clausula_id
                                                            FROM clausula_geral cgt
                                                            LEFT JOIN estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                                            LEFT JOIN grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
                                                            LEFT JOIN LATERAL (
                                                                select cgt.id_clau, 
                                                                   cgt.tex_clau,
                                                                   cgt.assunto_idassunto,
                                                                   ect.nome_clausula,
                                                                   gct.nome_grupo,
                                                                   cgt.numero_clausula,
                                                                   cgt.doc_sind_id_documento,
                                                                   cgt.estrutura_id_estruturaclausula,
                                                                   cgt.sinonimo_id
                                                                FROM clausula_geral cgt
                                                                LEFT JOIN estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                                                LEFT JOIN grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
                                                                where cgt.doc_sind_id_documento = @docIdAnterior and cgt.liberado = 'S' and (cgt.aprovado = 'Sim' or cgt.aprovado = 'sim') @grupoClausula @estruturaClausula
                                                            ) x on cgt.sinonimo_id = x.sinonimo_id 
                                                            where cgt.doc_sind_id_documento = @docIdReferencia and cgt.liberado = 'S' and (cgt.aprovado = 'Sim' or cgt.aprovado = 'sim') @grupoClausula @estruturaClausula
                                                            union all 
                                                            select null novo_clausula_id, 
                                                                   cgt.id_clau antigo_clausula_id,
                                                                   null novo_texto,
                                                                   cgt.tex_clau antigo_texto,
                                                                   null novo_assunto_id,
                                                                   cgt.assunto_idassunto antigo_assunto_id,
                                                                   null novo_nome_clausula,
                                                                   ect.nome_clausula antigo_nome_clausula,
                                                                   null novo_nome_grupo_clausula,
                                                                   gct.nome_grupo antigo_nome_grupo_clausula,
                                                                   null novo_numero_clausula,
                                                                   cgt.numero_clausula antigo_numero_clausula,
                                                                   null novo_documento_sindical_id,
                                                                   cgt.doc_sind_id_documento antigo_documento_sindical_id,
                                                                   null novo_estutura_clausula_id,
                                                                   cgt.estrutura_id_estruturaclausula antigo_estutura_clausula_id      
                                                                   FROM clausula_geral cgt
                                                                   LEFT JOIN estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                                                   LEFT JOIN grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                                                   where cgt.doc_sind_id_documento = @docIdAnterior and cgt.liberado = 'S' and (cgt.aprovado = 'Sim' or cgt.aprovado = 'sim') @grupoClausula @estruturaClausula
                                                                   and not exists(select 1 from clausula_geral cgt2
                                                                                  where cgt2.sinonimo_id = cgt.sinonimo_id
                                                                                  and cgt2.doc_sind_id_documento = @docIdReferencia)
                                                              ) p order by p.novo_numero_clausula is null, p.novo_numero_clausula asc");

                    var parameterTextos = new Dictionary<string, object>();

                    var sindPatronal = new List<ComparacaoClausulaSindPatronal>();

                    if (docs != null)
                    {
                        parameterTextos.Add("docIdAnterior", docs.DocAnterior!);
                        parameterTextos.Add("docIdReferencia", docs.DocReferencia!);
#pragma warning disable CS8604 // Possible null reference argument.
                        if (!string.IsNullOrEmpty(docs.SindPatronal))
                        {
                            var objectPatronal = JsonConvert.DeserializeObject<ComparacaoClausulaSindPatronal[]>(docs.SindPatronal);
                            sindPatronal = objectPatronal.ToList();
                        }
#pragma warning restore CS8604 // Possible null reference argument.
                    }

                    if (request.GrupoClausula != null && request.GrupoClausula.Any())
                    {
                        var listaGruposClausulas = string.Join(",", request.GrupoClausula);
                        queryTextos.Replace("@grupoClausula", $@"and gct.idgrupo_clausula in ( {listaGruposClausulas} )");
                    }
                    else
                    {
                        queryTextos.Replace("@grupoClausula", string.Empty);
                    }

                    if (request.EstruturaClausula != null && request.EstruturaClausula.Any())
                    {
                        var listaEstruturasClausulas = string.Join(",", request.EstruturaClausula);
                        queryTextos.Replace("@estruturaClausula", $@"and cgt.estrutura_id_estruturaclausula in ( {listaEstruturasClausulas} )");
                    }
                    else
                    {
                        queryTextos.Replace("@estruturaClausula", string.Empty);
                    }

                    var textosResult = await _context.SelectFromRawSqlAsync<dynamic>(queryTextos.ToString(), parameterTextos);

                    if (textosResult != null)
                    {
                        var textosObj = textosResult.Select(t => new ComparacaoClausulaItem
                        {
                            Clausula = t.novo_nome_clausula ?? t.antigo_nome_clausula,
                            Grupo = t.novo_nome_grupo_clausula ?? t.antigo_nome_grupo_clausula,
                            IdAssuntoReferencia = t.novo_assunto_id,
                            IdAssuntoAnterior = t.antigo_assunto_id,
                            TextoAnterior = t.velho_texto,
                            TextoReferencia = t.novo_texto
                        });

                        var newObj = new List<ComparacaoClausulaItem>();

                        foreach (var text in textosObj)
                        {
                            if (request.ExibirDiferencas)
                            {
                                var separatedAnteriorString = text.TextoAnterior?.Split(new[] { "\n" }, 2, StringSplitOptions.None);
                                var separatedReferenciaString = text.TextoReferencia?.Split(new[] { "\n" }, 2, StringSplitOptions.None);
                                var d = new Differ();
                                var builder = new SideBySideDiffBuilder(d);

                                if (separatedAnteriorString?.Length == 1 || separatedReferenciaString?.Length == 1)
                                {
                                    var newResult = builder.BuildDiffModel(separatedAnteriorString?[0] ?? string.Empty, separatedReferenciaString?[0] ?? string.Empty, false, true);
                                    text.Diferenca = newResult;
                                    newObj.Add(new ComparacaoClausulaItem
                                    {
                                        Diferenca = newResult,
                                        Clausula = text.Clausula,
                                        Grupo = text.Grupo,
                                        IdAssuntoAnterior = text.IdAssuntoAnterior,
                                        IdAssuntoReferencia = text.IdAssuntoReferencia,
                                        TextoAnterior = text.TextoAnterior,
                                        TextoReferencia = text.TextoReferencia,
                                        TextoSeparado = separatedReferenciaString,
                                    });

                                    continue;
                                }

                                var builderResult = builder.BuildDiffModel(separatedAnteriorString?[1] ?? string.Empty, separatedReferenciaString?[1] ?? string.Empty, false, true);

                                text.Diferenca = builderResult;
                                newObj.Add(new ComparacaoClausulaItem
                                {
                                    Diferenca = builderResult,
                                    Clausula = text.Clausula,
                                    Grupo = text.Grupo,
                                    IdAssuntoAnterior = text.IdAssuntoAnterior,
                                    IdAssuntoReferencia = text.IdAssuntoReferencia,
                                    TextoAnterior = separatedAnteriorString?[1],
                                    TextoReferencia = separatedReferenciaString?[1],
                                    TituloClausulaAnterior = separatedAnteriorString?[0],
                                    TituloClausulaReferencia = separatedReferenciaString?[0],
                                    TextoSeparado = separatedReferenciaString,
                                });
                            }
                            else
                            {
                                var separatedAnteriorString = text.TextoAnterior?.Split(new[] { "\n" }, 2, StringSplitOptions.None);
                                var separatedReferenciaString = text.TextoReferencia?.Split(new[] { "\n" }, 2, StringSplitOptions.None);
                                var d = new Differ();
                                var builder = new SideBySideDiffBuilder(d);

                                if (separatedAnteriorString?.Length == 1 || separatedReferenciaString?.Length == 1)
                                {
                                    var newResult = builder.BuildDiffModel(separatedAnteriorString?[0] ?? string.Empty, separatedReferenciaString?[0] ?? string.Empty, false, true);
                                    text.Diferenca = newResult;
                                    newObj.Add(new ComparacaoClausulaItem
                                    {
                                        Diferenca = newResult,
                                        Clausula = text.Clausula,
                                        Grupo = text.Grupo,
                                        IdAssuntoAnterior = text.IdAssuntoAnterior,
                                        IdAssuntoReferencia = text.IdAssuntoReferencia,
                                        TextoAnterior = text.TextoAnterior,
                                        TextoReferencia = text.TextoReferencia,
                                        TextoSeparado = separatedReferenciaString,
                                    });

                                    continue;
                                }

                                var titleResult = builder.BuildDiffModel(separatedAnteriorString?.FirstOrDefault() ?? string.Empty, separatedReferenciaString?.FirstOrDefault() ?? string.Empty, false, true);
                                var builderResult = builder.BuildDiffModel(separatedAnteriorString?[1] ?? string.Empty, separatedReferenciaString?[1] ?? string.Empty, false, true);

                                text.Diferenca = builderResult;
                                newObj.Add(new ComparacaoClausulaItem
                                {
                                    Diferenca = builderResult,
                                    DiferencaTitulo = titleResult,
                                    Clausula = text.Clausula,
                                    Grupo = text.Grupo,
                                    IdAssuntoAnterior = text.IdAssuntoAnterior,
                                    IdAssuntoReferencia = text.IdAssuntoReferencia,
                                    TextoAnterior = separatedAnteriorString?[1],
                                    TextoReferencia = separatedReferenciaString?[1],
                                    TituloClausulaAnterior = separatedAnteriorString?[0],
                                    TituloClausulaReferencia = separatedReferenciaString?[0],
                                    TextoSeparado = separatedReferenciaString,
                                });
                            }
                        }

                        var comparacaoResult = new ComparacaoClausulaViewModel
                        {
                            ComparacaoClausulaItems = newObj,
                        };

                        if (sindPatronal.Any() && sindPatronal != null)
                        {
                            var sindPatronalResult = await _context.SindPatrs.Where(x => x.Id == sindPatronal[0].Id).Select(x => new SindicatoPatronalViewModel
                            {
                                Id = x.Id,
                                Cnpj = x.Cnpj.Value,
                                Sigla = x.Sigla,
                                Uf = x.Uf,
                                RazaoSocial = x.RazaoSocial,
                                Municipio = x.Municipio
                            }).FirstOrDefaultAsync();

                            comparacaoResult = new ComparacaoClausulaViewModel
                            {
                                ComparacaoClausulaItems = newObj,
                                SindicatoPatronal = sindPatronalResult ?? null,
                            };
                        }

                        return Ok(comparacaoResult);
                    }
                }

                return NoContent();
            }

            return NotAcceptable();
        }

        [HttpPost]
        [Route("pdf-clausula")]
        [Produces(MediaTypeNames.Application.Pdf)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(File), MediaTypeNames.Application.Pdf)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Pdf)]
        public IActionResult GerarPdfAsync([FromBody] PdfClausulaRequest request)
        {
            IEnumerable<ClausulaViewModel> clausulasRecebidas = request.Clausulas ?? new List<ClausulaViewModel>();

            var html = new StringBuilder(@"
                <html>
                <head>
                    <style>
                        mark {
                            padding: 0;
                        }
            
                        .container {
                            display: flex;
                            flex-direction: column;
                        }

                        .clausula-title {
                            font-size: 1.6rem;
                        }

                        .clausula-location {
                            font-size: 1.25rem;
                            text-decoration: underline;
                        }

                        .header-flex {
                            display: flex;
                            flex-direction: row;
                            margin-bottom: 10px;
                            justify-content: space-between;
                            width: 100%;
                        }

                        .header-flex div {
                            margin-bottom: 10px;
                        }

                        body {
                            font-family: 'Montserrat', sans-serif;
                        }

                        .ineditta-top {
                            text-align: right;
                        }

                        hr {
                            border: 2px solid #000
                        }

                        .table-striped>tbody>tr:nth-of-type(odd) {
                            background-color: #f9f9f9
                        }

                        .table {
                            width: 100%;
                            max-width: 100%;
                        }

                        td {
                            line-height: 1.5;
                        }
                    </style>
                </head>
                <body>
                    <p class='ineditta-top'><strong>Ineditta Consultoria Sindical e Trabalhista Ltda</strong></p>");


            foreach (var clausula in clausulasRecebidas)
            {
                DateTime dataInicial = clausula.ValidadeInicial!.Value.ToDateTime(TimeOnly.MinValue);
                DateTime dataFinal = clausula.ValidadeFinal!.Value.ToDateTime(TimeOnly.MinValue);

                html.Append(CultureInfo.InvariantCulture, $@"
                <div class='container'>
                    <p class='clausula-location'><strong>{clausula.SindLaboral!.First().Uf} / {clausula.SindLaboral!.First().Municipio}</strong></p>
                    <table class='table'>
                        <tbody>
                            <tr>
                                <td><strong>Cláusula:</strong> {clausula.NomeClausula}</td>
                                <td><strong>Grupo:</strong> {clausula.GrupoClausula}</td>
                            </tr>
                            <tr>
                                <td><strong>Documento:</strong> {clausula.NomeDocumento}</td>
                                <td><strong>Data Base:</strong> {clausula.DataBase}</td>
                            </tr>
                            <tr>
                                <td><strong>Atividade Econômica:</strong> {string.Join('/', clausula.Cnae!.Select(obj => obj.Subclasse)) ?? string.Empty}</td>
                            </tr>
                            <tr>
                                <td><strong>Sindicato Laboral:</strong> {string.Join("; ", clausula.SindLaboral!.Select(obj => obj.Sigla + " / " + obj.Denominacao)) ?? string.Empty}</td>
                            </tr>
                            <tr>
                                <td><strong>Sindicato Patronal:</strong> {string.Join("; ", clausula.SindPatronal!.Select(obj => obj.Sigla + " / " + obj.Denominacao)) ?? string.Empty}</td>
                            </tr>
                        </tbody>
                    </table>

                    <table class='table' style='margin-bottom: 20px;'>
                        <tbody>
                            <tr>
                                <td><strong>Estado:</strong> {string.Join('/', clausula.SindLaboral!.Select(obj => obj.Uf)) ?? string.Empty}</td>
                                <td><strong>Cidade:</strong> {string.Join('/', clausula.SindLaboral!.Select(obj => obj.Municipio)) ?? string.Empty}</td>
                                <td><strong>Período:</strong> {dataInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - {dataFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}</td>
                            </tr>
                        </tbody>
                    </table>

                    <div>
                        <p><strong>Texto</strong>

                        <p style='text-align: justify; white-space: pre-line'>{clausula.TextoClausula}</p>");

                if (clausula.Comentarios != null && clausula.Comentarios.Any())
                {
                    html.Append(@"<table class='table table-striped' style='margin-bottom: 20px;'>
                                        <thead>
                                            <tr >
                                                <th>Usuário</th>
                                                <th>Data</th>
                                                <th>Etiqueta</th>
                                                <th>Comentário</th>
                                            </tr>
                                        </thead>
                                        <tbody>");
#pragma warning disable CS8602 // Possible null reference argument.
                    foreach (var comentario in clausula?.Comentarios)
                    {
#pragma warning disable CA1305 // Specify IFormatProvider
                        html.Append($@"<tr>
                                            <td>{comentario.NomeUsuario ?? string.Empty}</td>");

                        if (comentario.DataRegistro != null)
                        {
                            DateTime date = (DateTime)comentario.DataRegistro;

                            html.Append(CultureInfo.InvariantCulture, $@"<td style='text-align: center'>{date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}</td>");
                        }

                        html.Append(CultureInfo.InvariantCulture, $@"<td>{comentario.Etiqueta ?? string.Empty}</td>
                                        <td>{comentario.Comentario ?? string.Empty}</td>
                                    </tr>");
#pragma warning restore CA1305 // Specify IFormatProvider
                    }

                    html.Append(@"</tbody>
                                </table>");
                }
#pragma warning restore CS8602 // Possible null reference argument.

                html.Append(@"<hr>
                    </div>
                </div>");
            }

            html.Append(@"</body>");

            var file = Encoding.UTF8.GetBytes(html.ToString());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10, Left = 20, Right = 20 }
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        HtmlContent = Encoding.UTF8.GetString(file),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return File(_converter.Convert(doc), "application/pdf", "relatorio_clausulas.pdf");
        }

        [HttpPost]
        [Route("pdf-comparacao")]
        [Produces(MediaTypeNames.Application.Pdf)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(File), MediaTypeNames.Application.Pdf)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Pdf)]
        public IActionResult GerarPdfComparativoAsync([FromBody] PdfComparacaoRequest request)
        {
            var html = new StringBuilder(@"
                <html>
                <head>
                    <style>
                        .container {
                            display: flex;
                            flex-direction: column;
                        }

                        .clausula-title {
                            font-size: 1.2rem;
                            color: #FFFFFF;
                        }


                        .header-flex div {
                            margin-bottom: 10px;
                        }

                        body {
                            font-family: 'Montserrat', sans-serif;
                        }

                        .ineditta-top {
                            text-align: right;
                        }

                        .clausula-title-container {
                            background-color: #4f8edc;
                            width: 100%;
                            padding: 0.5rem 0;
                            margin-bottom: 1rem;
                        }

                        hr {
                            border: 2px solid #000
                        }

                        .diff-row {
                            vertical-align: top;
                            width: 50%;
                            padding: 1rem 0.5rem;
                        }

                        .clausula_box {
                            box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
                            -webkit-box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
                            -moz-box-shadow: -4px -4px 0px 0px rgba(122, 122, 122, 0.66);
                            height: fit-content;
                            padding-right: 4px;
                            padding-left: 0;
                        }

                        .clausula_box_added {
                            box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
                            -webkit-box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
                            -moz-box-shadow: -4px -4px 0px 0px rgba(98, 240, 98, 1);
                            height: fit-content;
                            padding-right: 4px;
                            padding-left: 0;
                        }

                        .clausula_box_removed {
                            box-shadow: -4px -4px 0px 0px rgba(250, 145, 145, 1);
                            -webkit-box-shadow: -4px -4px 0px 0px rgba(250, 145, 145, 1);
                            height: fit-content;
                            padding-right: 4px;
                            padding-left: 0;
                        }

                        .clausula_null {
                            color: #8a6d3b;
                            background-color: #fcf8e3;
                            border-color: #faebcc;
                            border-radius: 4px;
                            height: fit-content;
                            text-align: center;
                        }

                        mark {
                            padding: 0;
                        }

                        .title-coluna-nova {
                            padding: 0.5rem 0 0.5rem 0;
                            margin-bottom: 1rem
                        }

                        .title-coluna-antiga {
                            padding: 0.5rem 0 0.5rem 0;
                            margin-bottom: 1rem
                        }

                        h5, h4 {
                            margin-left: 1rem;
                        }
                    </style>
                </head>
                <body>
                    <p class='ineditta-top'><strong>Ineditta Consultoria Sindical e Trabalhista Ltda</strong></p>");

            html.Append(CultureInfo.InvariantCulture, $@"<p><strong>Total de {request.DadosComparacao?.Count()} cláusulas visíveis.</strong></p>");

            if (request.DadosComparacao != null && request.DadosComparacao.Any())
            {
                foreach (var compara in request.DadosComparacao)
                {
                    html.Append(CultureInfo.InvariantCulture, $@"
                                <div class='clausula-title-container'>
                                    <h4 class='clausula-title'><strong>Cláusula:</strong> {compara.NomeClausula} - <strong>Grupo:</strong> {compara.GrupoClausula}</h4>
                                </div>");

                    html.Append(CultureInfo.InvariantCulture, $@"
                                <table>
                                    <tbody>
                                        <tr>
                                            <td class='diff-row'>
                                                {compara.HtmlDiferencasNovo}
                                            </td>
                                            <td class='diff-row'>
                                                {compara.HtmlDiferencasAntigo}
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                ");

                    html.Append(CultureInfo.InvariantCulture, $@"
                    <div class='container'>
                        
                    </div>");
                }
            }
            html.Append(@"</body>");

            var htmlString = html.ToString();

            var file = Encoding.UTF8.GetBytes(htmlString);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10, Left = 5, Right = 5 }
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        HtmlContent = Encoding.UTF8.GetString(file),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return File(_converter.Convert(doc), "application/pdf", "comparativo_clausulas.pdf");
        }

        [HttpGet]
        [Route("sind-patronal-select")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterSindPatronalPorLaboralAsync([FromQuery] SindPatronalComparativoRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new Dictionary<string, object>();

                var query = new StringBuilder(@"select id_sindp id, cnpj_sp cnpj, sigla_sp sigla, razaosocial_sp razao  from sind_patr sp
                                                inner join doc_sind doc on JSON_CONTAINS(doc.sind_patronal, JSON_OBJECT('id', sp.id_sindp))
                                                where JSON_CONTAINS(doc.sind_laboral, JSON_OBJECT('id', @sindLaboral)) @grupoEconomicoString
                                                group by id_sindp ");

                if (request.SindLaboral > 0)
                {
                    parameters.Add("sindLaboral", request.SindLaboral);
                }

                if (request.GrupoEconomicoId > 0)
                {
                    query.Replace("@grupoEconomicoString", $@"and JSON_CONTAINS(doc.cliente_estabelecimento, JSON_OBJECT('g', {request.GrupoEconomicoId}))");
                }
                else
                {
                    query.Replace("@grupoEconomicoString", string.Empty);
                }

                var result = await _context.SelectFromRawSqlAsync<dynamic>(query.ToString(), parameters);

                if (result != null)
                {
                    return Ok(result
                        .Select(x => new OptionModel<int>
                        {
                            Id = x.id,
                            Description = $@"{x.sigla} / {x.cnpj} / {x.razao}",
                        }));
                }

                return NoContent();
            }

            return NotAcceptable();
        }

        [HttpPost]
        [Route("relatorios")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> GerarExcelClausulasAsync([FromBody] RelatorioClausulaExcelRequest request)
        {
            const int id_modulo_cadastro_comentario = 7;
            const int id_modulo_clausulas = 6;

            var usuario = _httpRequestItemService.ObterUsuario();
            if (usuario == null)
            {
                return BadRequestFromErrorMessage("Usuário não identificado.");
            }

            var permissoesModuloCadastroComentarios = usuario.ModulosComerciais is not null ? 
                                                        usuario.ModulosComerciais.SingleOrDefault(m => m.Id == id_modulo_cadastro_comentario) : null;

            var permissoesModuloClausulas = usuario.ModulosComerciais is not null ?
                                              usuario.ModulosComerciais.SingleOrDefault(m => m.Id == id_modulo_clausulas) : null;

            var podeVisualizarComentariosClausulas = permissoesModuloCadastroComentarios is not null &&
                                                     permissoesModuloClausulas is not null &&
                                                     permissoesModuloCadastroComentarios.Consultar && 
                                                     permissoesModuloClausulas.Consultar;

            if (request.ClausulasIds is null || !request.ClausulasIds.Any())
            {
                return BadRequestFromErrorMessage("Você deve fornecer ao menos uma cláusula para obter o relatório");
            }

            var userEmail = _userInfoService.GetEmail();
            if (userEmail == null) return BadRequestFromErrorMessage("Usuário não identificado ou sem email");

            var parameters = new Dictionary<string, object> { 
                { "@userEmail", userEmail },
                { "@podeVisualizarComentariosClausula", podeVisualizarComentariosClausulas }
            };

            var query = new StringBuilder(@"
                SELECT 
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
                    ds.abrangencia abrangencia_documento_string,
	                ec.nome_clausula nome_clausula,
	                gc.nome_grupo nome_grupo_clausula,
	                cg.numero_clausula numero_clausula,
	                cg.tex_clau texto_clausula,
                    referencias.nomes referencias_documento,
                    comentarios.resumo comentarios_clausula
                FROM clausula_geral cg 
                LEFT JOIN estrutura_clausula ec ON ec.id_estruturaclausula = cg.estrutura_id_estruturaclausula
                LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula
                LEFT JOIN doc_sind ds ON ds.id_doc = cg.doc_sind_id_documento
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc 
                LEFT JOIN LATERAL (
                    SELECT 
                        GROUP_CONCAT(DISTINCT cut.codigo_unidade separator ', ') AS codigos, 
                        GROUP_CONCAT(DISTINCT cut.cnpj_unidade separator ', ') AS cnpjs,
                        GROUP_CONCAT(DISTINCT cut.cod_sindcliente separator ', ') AS codigos_sindicato_cliente,
                        GROUP_CONCAT(DISTINCT l.uf separator ', ') AS ufs,
                        GROUP_CONCAT(DISTINCT l.municipio separator ', ') AS municipios
                    FROM cliente_unidades cut
                    INNER JOIN usuario_adm ua on ua.email_usuario = @userEmail
                    INNER JOIN doc_sind dsindt2 on ds.id_doc = dsindt2.id_doc
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
                LEFT JOIN LATERAL (
	                SELECT
		                GROUP_CONCAT(ec2.nome_clausula separator ', ') AS nomes
	                FROM estrutura_clausula ec2 
	                WHERE JSON_CONTAINS(ds.referencia, JSON_ARRAY(ec2.id_estruturaclausula)) 
		                  OR JSON_CONTAINS(ds.referencia, JSON_ARRAY(CAST(ec2.id_estruturaclausula AS CHAR)))
                ) referencias ON true
                LEFT JOIN LATERAL (
	                SELECT 
		                GROUP_CONCAT(CONCAT(usuario_criacao.nome_usuario,' - ',DATE_FORMAT(ct.data_inclusao, '%d-%m-%Y %H:%i'),' - ',et.nome,' - ',ct.valor) separator '; ') resumo
	                FROM comentarios_tb ct
	                INNER JOIN usuario_adm usuario_criacao on usuario_criacao.id_user = ct.usuario_inclusao_id
	                INNER JOIN usuario_adm usuario_requerente on usuario_requerente.email_usuario = @userEmail
	                LEFT JOIN etiqueta_tb et ON et.id = ct.etiqueta_id
	                WHERE ct.tipo = 1
	                      AND ct.referencia_id = cg.id_clau
	                      AND CASE WHEN usuario_requerente.id_user = usuario_criacao.id_user THEN TRUE
	                               ELSE ct.visivel = 1
	                          END
	                      AND CASE WHEN usuario_requerente.nivel = 'Ineditta' THEN TRUE
	                               WHEN usuario_requerente.nivel = 'Grupo Econômico' THEN usuario_criacao.id_grupoecon = usuario_requerente.id_grupoecon
	                               ELSE JSON_OVERLAPS(usuario_criacao.ids_fmge, usuario_requerente.ids_fmge)
	                          END
                ) comentarios ON @podeVisualizarComentariosClausula
                WHERE TRUE 
            ");

            QueryBuilder.AppendListToQueryBuilder(query, request.ClausulasIds, "cg.id_clau", parameters);

            var result = await _context.SelectFromRawSqlAsync<RelatorioClausulasExcelInfoViewModel>(query.ToString(), parameters);

            if (result is null) return NoContent();

            var bytesRelatorio = RelatorioClausulasExcelFactory.Create(result);

            return bytesRelatorio == null ? NoContent() : DownloadExcel("mapa-sindical", bytesRelatorio);
        }
    }
}
