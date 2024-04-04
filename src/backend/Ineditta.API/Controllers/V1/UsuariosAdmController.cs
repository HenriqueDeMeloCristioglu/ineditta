using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.Application.Usuarios.UseCases.Upsert;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.Usuarios.UseCases.AtualizacaoCredenciais;
using Ineditta.Application.Usuarios.UseCases.AtualizarPermissoes;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Usuarios.UseCases.EnviarEmailBoasVindas;
using MySqlConnector;
using System.Text;
using Ineditta.Repository.Extensions;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.API.ViewModels.Usuariosadm.Requests;
using Ineditta.API.ViewModels.Usuariosadm.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/usuario-adms")]
    [ApiController]
    public class UsuariosAdmController : ApiBaseController
    {
#pragma warning disable S4487
        private readonly InedittaDbContext _context;
        private readonly IAuthService _authService;
        private readonly IUserInfoService _userInfoService;
        private readonly HttpRequestItemService _httpRequestItemService;

        public UsuariosAdmController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IAuthService authService, IUserInfoService userInfoService, HttpRequestItemService httpRequestItemService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _authService = authService;
            _userInfoService = userInfoService;
            _httpRequestItemService = httpRequestItemService;
        }



        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<UsuariosAdmViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();

                var query = @"select vw.* from usuarios_adms_vw vw
		                            inner join usuario_adm ult on ult.email_usuario = @email
                                    where case when ult.nivel = 'Ineditta' then true
                                    when ult.nivel = 'Grupo Econômico' then ult.id_grupoecon = vw.id_grupoecon and vw.nivel <> 'Ineditta' 
                                    else vw.id_grupoecon = ult.id_grupoecon and vw.nivel not in ('Ineditta', 'Grupo Econômico') and JSON_OVERLAPS(vw.ids_fmge, ult.ids_fmge) > 0 and case when ult.nivel = 'Estabelecimento' then ult.nivel = vw.nivel else true end
                                    end";

                parameters.Add(new MySqlParameter("email", _userInfoService.GetEmail()!));

                var result = await _context.UsuariosAdmsVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(us => new UsuariosAdmViewModel
                    {
                        Id = us.Id,
                        Nome = us.Nome,
                        Email = us.Email,
                        Cargo = us.Cargo,
                        Telefone = us.Telefone,
                        Ramal = us.Ramal,
                        Departamento = us.Departamento,
                        IdSuperior = us.IdSuperior,
                        DataCriacao = us.DataCriacao,
                        NomeUserCriador = us.NomeUserCriador == null ? default : us.NomeUserCriador
                    }).ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(UsuariosAdmViewModel), MediaTypeNames.Application.Json)]
        [Route("permissoes")]
        public async ValueTask<IActionResult> ObterTodosPorGrupoEconomicoAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                { "email", _userInfoService.GetEmail()! }
            };

            var result = await _context.SelectFromRawSqlAsync<dynamic>(@"select uat.id_user usuario_id,
                                            umst.*,
                                            mt.modulos,
                                            '1' as tipo,
                                            'SISAP' as tipo_nome
                                            from usuario_adm uat
                                            join json_table(uat.modulos_sisap, '$[*]' columns (
	                                            modulo_id int4 PATH '$.id',
	                                            consultar varchar(1) PATH '$.Consultar',
	                                            criar varchar(1) PATH '$.Criar',
	                                            alterar varchar(1) PATH '$.Alterar',
	                                            aprovar varchar(1) PATH '$.Aprovar',
	                                            excluir varchar(1) PATH '$.Excluir',
	                                            comentar varchar(1) PATH '$.Comentar'
                                            )) as umst on true
                                            left join modulos mt 
	                                            on umst.modulo_id = mt.id_modulos 
                                            where uat.email_usuario = @email
                                            union all 
                                            select uat.id_user,
                                            umst.*,
                                            mt.modulos,
                                            '2' as tipo,
                                            'COMERCIAL' as tipo_nome
                                            from usuario_adm uat
                                            join json_table(uat.modulos_comercial , '$[*]' columns (
	                                            modulo_id int4 PATH '$.id',
	                                            consultar varchar(1) PATH '$.Consultar',
	                                            criar varchar(1) PATH '$.Criar',
	                                            alterar varchar(1) PATH '$.Alterar',
	                                            aprovar varchar(1) PATH '$.Aprovar',
	                                            excluir varchar(1) PATH '$.Excluir',
	                                            comentar varchar(1) PATH '$.Comentar'
                                            )) as umst on true
                                            left join modulos mt 
	                                            on umst.modulo_id = mt.id_modulos 
                                            where uat.email_usuario = @email", parameters);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }


        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<UsuariosAdmViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("grupo-economico")]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterTodosPorGrupoEconomicoAsync([FromQuery] UsuariosPorGrupoEconomicoRequest request)
        {
            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null) return Unauthorized();

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                if (usuario.Nivel == Nivel.Ineditta)
                {
#pragma warning disable S3358 // Ternary operators should not be nested
                    var resultIneditta = await (from us in _context.UsuarioAdms
                                                where !request.FiltrarUsuariosAceitamNotificacaoEmail || us.NotificarEmail
                                                select new UsuarioGrupoEconomicoViewModel
                                                {
                                                    Id = us.Id,
                                                    Nome = us.Nome,
                                                    Departamento = us.Departamento,
                                                    Nivel = us.Nivel == Nivel.Ineditta ? "Ineditta" :
                                                            us.Nivel == Nivel.GrupoEconomico ? "Grupo Econômico" :
                                                            us.Nivel == Nivel.Matriz ? "Matriz" : "Estabelecimento"

                                                })
                                 .ToDataTableResponseAsync(request);
#pragma warning restore S3358 // Ternary operators should not be nested

                    return Ok(resultIneditta);
                }

                List<int> estabelecimentosIdsList = request.EstabelecimentosIds?.ToList() ?? new List<int>();

                var parameters = new List<MySqlParameter>();
                var parameterCount = 0;

                var query = new StringBuilder(@"
                    SELECT * FROM usuario_adm uad
                    WHERE CASE WHEN (uad.nivel = @ineditta OR uad.nivel = @grupoEconomico) THEN uad.id_grupoecon = @grupoEconomicoId
                               ELSE uad.id_grupoecon = @grupoEconomicoId 
                         
                ");

                var queryFiltroSindicatos = new StringBuilder(@"
                    EXISTS (
	                    SELECT 1
	                    FROM cliente_unidades cut
	                    LEFT JOIN LATERAL (
	                        SELECT ce.classe_cnae_idclasse_cnae cnaes, ce.cliente_unidades_id_unidade
	                        FROM cnae_emp ce
	                    ) cc ON cc.cliente_unidades_id_unidade = cut.id_unidade
	                    LEFT JOIN base_territorialsindemp bt
	                        ON bt.classe_cnae_idclasse_cnae = cc.cnaes
	                        AND bt.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
	                    LEFT JOIN base_territorialsindpatro bt2 
		                    ON bt2.classe_cnae_idclasse_cnae = cc.cnaes
	                        AND bt2.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
                        WHERE CASE WHEN uad.nivel = 'Ineditta' THEN TRUE 
	           	                    WHEN uad.nivel = 'Grupo Econômico' THEN cut.cliente_grupo_id_grupo_economico = uad.id_grupoecon
	                                ELSE JSON_CONTAINS(uad.ids_fmge, JSON_ARRAY(cut.id_unidade))
	                          END 
                    
                ");

                if (estabelecimentosIdsList is not null && estabelecimentosIdsList.Any())
                {
                    QueryBuilder.AppendListArrayToQueryBuilder(query, estabelecimentosIdsList, "uad.ids_fmge", parameters, ref parameterCount);
                }

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryFiltroSindicatos, request.SindicatosLaboraisIds, "bt.sind_empregados_id_sinde1", parameters, ref parameterCount);
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(queryFiltroSindicatos, request.SindicatosPatronaisIds, "bt2.sind_patronal_id_sindp", parameters, ref parameterCount);
                }

                query.Append(@" END");

                if (request.FiltrarUsuariosAceitamNotificacaoEmail)
                {
                    query.Append(@" AND uad.notifica_email = 1");
                }

                if ((request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any()) || (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any()))
                {
                    queryFiltroSindicatos.Append(@" )");
                    query.Append(@" AND ");
                    query.Append(queryFiltroSindicatos);
                }

                parameters.Add(new MySqlParameter("@ineditta", Nivel.Ineditta.GetDescription()));
                parameters.Add(new MySqlParameter("@grupoEconomico", Nivel.GrupoEconomico.GetDescription()));
                parameters.Add(new MySqlParameter("@grupoEconomicoId", usuario.GrupoEconomicoId));

#pragma warning disable S3358 // Ternary operators should not be nested
                var queryExecute = _context.UsuarioAdms.FromSqlRaw(query.ToString(), parameters.ToArray())
                                   .Select(uad => new UsuarioGrupoEconomicoViewModel
                                   {
                                       Id = uad.Id,
                                       Nome = uad.Nome,
                                       Departamento = uad.Departamento,
                                       Nivel = uad.Nivel == Nivel.Ineditta ? "Ineditta" :
                                               uad.Nivel == Nivel.GrupoEconomico ? "Grupo Econômico" :
                                               uad.Nivel == Nivel.Matriz ? "Matriz" : "Estabelecimento"
                                   });
#pragma warning restore S3358 // Ternary operators should not be nested

                var result = await queryExecute.ToDataTableResponseAsync(request);



                return Ok(result);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertUsuarioRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [Route("{id:int}")]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertUsuarioRequest request)
        {
            if (request is null)
            {
                return EmptyRequestBody();
            }

            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Route("{id:int}")]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            var result = await (from us in _context.UsuarioAdms
                                join usst in _context.UsuarioAdms on (long?)us.IdSuperior equals usst.Id into _usst
                                from usst in _usst.DefaultIfEmpty()
                                join jt in _context.Jornada on (long?)us.JornadaId equals jt.Id into _jt
                                from jt in _jt.DefaultIfEmpty()
                                join gct in _context.GrupoEconomico on us.GrupoEconomicoId equals gct.Id into _gct
                                from gct in _gct.DefaultIfEmpty()
                                where us.Id == id
                                select new UsuarioViewModel
                                {
                                    Id = us.Id,
                                    Nome = us.Nome,
                                    Email = us.Email.Valor,
                                    Cargo = us.Cargo,
                                    Celular = us.Celular,
                                    Ramal = us.Ramal,
                                    Departamento = us.Departamento,
                                    Nivel = us.Nivel,
                                    AusenciaFim = us.Ausencia == null || us.Ausencia!.DataInicial.Year <= 1900 ? null : us.Ausencia!.DataFinal,
                                    AusenciaInicio = us.Ausencia == null || us.Ausencia!.DataInicial.Year <= 1900 ? null : us.Ausencia!.DataInicial,
                                    Bloqueado = us.Bloqueado,
                                    Foto = us.Foto,
                                    NotificarEmail = us.NotificarEmail,
                                    DocumentoRestrito = us.DocumentoRestrito,
                                    NotificarWhatsapp = us.NotificarWhatsapp,
                                    TrocaSenhaLogin = us.TrocaSenhaLogin,
                                    Tipo = us.Tipo,
                                    Superior = usst == null ? default : new OptionModel<long>
                                    {
                                        Id = usst.Id,
                                        Description = usst.Nome
                                    },
                                    Jornada = jt == null ? default : new OptionModel<long>
                                    {
                                        Id = jt.Id,
                                        Description = jt.Descricao
                                    },
                                    LocalidadesIds = us.LocalidadesIds,
                                    CnaesIds = us.CnaesIds,
                                    GruposClausulasIds = us.GruposClausulasIds,
                                    GrupoEconomico = gct == null ? default : new OptionModel<long>
                                    {
                                        Id = gct.Id,
                                        Description = gct.Nome
                                    },
                                    UsuarioModulosComerciais = us.ModulosComerciais,
                                    UsuarioModulosSisap = us.ModulosSISAP,
                                    EstabelecimentosIds = us.EstabelecimentosIds,
                                    UsuariosTiposEventosCalendario = _context.UsuariosTiposEventosCalendarioSindical
                                                                        .Where(utecs => utecs.UsuarioId == us.Id)
                                                                        .Select(utecs => new UsuarioTipoEventoCalendarioSindicalViewModel
                                                                        {
                                                                            Id = utecs.Id,
                                                                            UsuarioId = utecs.UsuarioId,
                                                                            Tipo = utecs.TipoEventoId,
                                                                            Subtipo = utecs.SubtipoEventoId,
                                                                            NotificarEmail = utecs.NotificarEmail,
                                                                            NotificarWhatsapp = utecs.NotificarWhatsapp,
                                                                            NotificarAntes = utecs.NotificarAntes.Days
                                                                        }).ToList()
                                })
                                .SingleOrDefaultAsync();

            if (result is null)
            {
                return NotFound(Errors.General.NotFound());
            }

            if (result.LocalidadesIds is not null)
            {
                FormattableString localizacaoQuery = $@"select cc.* from ineditta.localizacao cc 
                            inner join lateral(
                            select cod_uf, max(id_localizacao) maior_id
                            from ineditta.localizacao
                            group by 1) cct 
                            where cct.maior_id = cc.id_localizacao";

#pragma warning disable CA1305 // Specify IFormatProvider
                result.Localidades = await _context.Localizacoes.FromSql(localizacaoQuery).Where(lt => result.LocalidadesIds.Contains(Convert.ToInt32(EF.Property<string>(lt, "CodUf")))).Select(lt => new OptionModel<int>
                {
                    Id = lt.Id,
                    Description = lt.Uf.Id
                }).ToListAsync();
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            if (result.CnaesIds is not null)
            {
                FormattableString cnaeQuery = $@"select cc.* from ineditta.classe_cnae cc 
                            inner join lateral(
                            select divisao_cnae, max(id_cnae) maior_id
                            from ineditta.classe_cnae
                            group by 1) cct 
                            where cct.maior_id = cc.id_cnae
                ";

                result.Cnaes = await _context.ClasseCnaes.FromSql(cnaeQuery).Where(cct => result.CnaesIds.Contains(cct.Divisao)).Select(cct => new OptionModel<int>
                {
                    Id = cct.Divisao,
                    Description = cct.DescricaoDivisao
                }).ToListAsync();
            }

            if (result.GruposClausulasIds is not null)
            {
                result.GruposClausulas = await (from gcs in _context.GrupoClausula
                                                where result.GruposClausulasIds.Contains(gcs.Id)
                                                select new OptionModel<int>
                                                {
                                                    Id = gcs.Id,
                                                    Description = gcs.Nome
                                                }).ToListAsync();
            }

            if (result.EstabelecimentosIds is not null)
            {
                result.GruposEconomicosConsultores = (await (from cgt in _context.GrupoEconomico
                                                             join cut in _context.ClienteUnidades on cgt.Id equals EF.Property<int>(cut, "GrupoEconomicoId")
                                                             where result.EstabelecimentosIds.Contains(cut.Id)
                                                             select new OptionModel<int>
                                                             {
                                                                 Id = cgt.Id,
                                                                 Description = cgt.Nome
                                                             })
                                                            .ToListAsync())?.DistinctBy(p => p.Id);
            }
#pragma warning disable S125
            var usuarioAuth = await _authService.ObterPorEmailAsync(result.Email!);

            if (usuarioAuth.IsFailure)
            {
                return NotFound(Errors.General.NotFound());
            }

            result.Username = usuarioAuth.Value.Username;
#pragma warning restore S125
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Route("{id:int}/email-atualizacoes-cadastrais")]
        public async ValueTask<IActionResult> EnviarEmailAtualizacaoCadastral([FromRoute] int id)
        {
            var request = new EnviarEmailAtualizacaoCredenciaisRequest { Id = id };

            return await Dispatch(request);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Route("email-boas-vindas")]
        public async ValueTask<IActionResult> EnviarEmailBoasVindas([FromQuery] string email, [FromHeader(Name = "X-request-ID")] Guid requestId)
        {
            var request = new EnviarEmailBoasVindasRequest { Email = email, IdempotentToken = requestId };

            return await Dispatch(request);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Route("{id:int}/permissoes")]
        public async ValueTask<IActionResult> AtualizarPemissoesAsync([FromRoute] int id)
        {
            var request = new AtualizarPermissaoRequest { Id = id };

            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<long>>), SelectMediaType.ContentType)]
        [Route("ineditta")]
        public async ValueTask<IActionResult> ObterTiposDocsAsync()
        {
            return Request.Headers.Accept == SelectMediaType.ContentType
                ? Ok(await _context.UsuarioAdms
                            .AsNoTracking()
                            .Where(ua => ua.Tipo == "Ineditta")
                            .Select(ua => new OptionModel<long>
                            {
                                Id = ua.Id,
                                Description = ua.Nome,
                            })
                            .ToListAsync())
                : NoContent();
        }

        [HttpGet("dados-pessoais")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(UsuarioDadosPessoaisViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterDadosPessoaisAsync()
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var dadosPessoais = await _context.UsuarioAdms.Where(uat => uat.Email.Valor == _userInfoService.GetEmail())
                .Select(uat => new UsuarioDadosPessoaisViewModel
                {
                    Id = uat.Id,
                    Nivel = uat.Nivel,
                    GrupoEconomicoId = uat.GrupoEconomicoId
                })
                .SingleOrDefaultAsync();

            return dadosPessoais is null ? NotFound(Errors.General.NotFound()) : Ok(dadosPessoais);
        }

        [HttpGet("documentos/{documentoId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<UsuariosPorDocumentoDataTableViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosPorDocumentoAsync([FromRoute] int documentoId, [FromQuery] DataTableRequest request, [FromQuery] int modulo)
        {
            var parameters = new List<MySqlParameter>();

            var query = new StringBuilder(@"
                SELECT * FROM usuario_adm ua,
                json_table(ua.modulos_comercial, '$[*]' COLUMNS (id INT PATH '$.id', consultar varchar(1) path '$.Consultar')) uatmdt
                WHERE EXISTS (
	                SELECT 1 FROM doc_sind ds 
	                WHERE ds.id_doc = @documentoId
	                      AND CASE WHEN ua.nivel = 'Grupo Econômico' THEN JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"": ', ua.id_grupoecon,'}}'))
	                               ELSE EXISTS (
      		                            SELECT 1 FROM cliente_unidades cut
      		               	            WHERE EXISTS (
      		               		            SELECT 
								                jt.*
								            FROM 
								                JSON_TABLE(
								                    ds.cliente_estabelecimento,
								                    ""$[*]"" COLUMNS (
								                        g INT PATH ""$.g"",
								                        m INT PATH ""$.m"",
								                        u INT PATH ""$.u"",
								                        nome_unidade VARCHAR(255) PATH ""$.nome_unidade""
								                    )
								                ) AS jt
								             WHERE jt.u = cut.id_unidade AND JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
      		               	            )	
                                  )
	                          END
                ) AND ua.nivel <> 'Ineditta'
                AND ua.notifica_email = 1
                and uatmdt.id = @modulo
                AND ua.is_blocked = 0
            ");

            parameters.Add(new MySqlParameter("@modulo", modulo));
            parameters.Add(new MySqlParameter("@documentoId", documentoId));

#pragma warning disable S3358 // Ternary operators should not be nested
                var usuarios = await _context.UsuarioAdms.FromSqlRaw(query.ToString(), parameters.ToArray())
                .Select(u => new UsuariosPorDocumentoDataTableViewModel
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email.Valor,
                    Nivel = u.Nivel == Nivel.Ineditta ? "Ineditta" : u.Nivel == Nivel.GrupoEconomico ? "Grupo Econômico" : u.Nivel == Nivel.Matriz ? "Matriz" : "Unidade"
                })
                .AsNoTracking()
                .ToDataTableResponseAsync(request);
#pragma warning restore S3358 // Ternary operators should not be nested

            if (usuarios is null)
            {
                return NoContent();
            }

            return Ok(usuarios);
        }
    }
}
