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
using Ineditta.API.ViewModels.Localizacoes;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.Application.Usuarios.Entities;
using MySqlConnector;
using System.Text;
using Ineditta.Repository.Extensions;
using Ineditta.API.ViewModels.Localizacoes.Requests;
using Ineditta.API.ViewModels.Localicacoes.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/localizacoes")]
    [ApiController]
    public class LocalizacaoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public LocalizacaoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(LocalizacaoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            var result = await (from lt in _context.Localizacoes
                                select new LocalizacaoViewModel
                                {
                                    Id = lt.Id,
                                    CodigoPais = EF.Property<string>(lt,"CodPais"),
                                    Pais = lt.Pais.Id,
                                    CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                                    CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                                    CodigoUf = EF.Property<string>(lt, "CodUf"),
                                    Estado = lt.Estado.Id,
                                    Uf = lt.Uf.Id,
                                    Municipio = lt.Municipio,
                                    Regiao = lt.Regiao.Id,
                                }).SingleOrDefaultAsync(ltw => ltw.Id == id);

            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpGet("ufs/{uf}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(LocalizacaoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterTodosPorUfAsync([FromQuery] DataTableRequest request, [FromRoute]string? uf)
        {
            var result = await (from lt in _context.Localizacoes
                                where lt.Uf.Id == null || lt.Uf.Id == uf
                                select new LocalizacaoViewModel
                                {
                                    Id = lt.Id,
                                    CodigoPais = EF.Property<string>(lt, "CodPais"),
                                    Pais = lt.Pais.Id,
                                    CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                                    CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                                    CodigoUf = EF.Property<string>(lt, "CodUf"),
                                    Estado = lt.Estado.Id,
                                    Uf = lt.Uf.Id,
                                    Municipio = lt.Municipio,
                                    Regiao = lt.Regiao.Id,
                                })
                                .AsNoTracking()
                                .ToDataTableResponseAsync(request);

            return result == null ?
                NoContent() :
                Ok(result);
        }

        [HttpPost("obter-por-lista-ids")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(LocalizacaoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorListaIdsAsync([FromBody] LocalizacaoDatatablePorListaIdsRequest request)
        {
            var result = await (from lt in _context.Localizacoes
                                where request.AbrangenciasSelecionadas.Contains(lt.Id)
                                select new LocalizacaoViewModel
                                {
                                    Id = lt.Id,
                                    CodigoPais = EF.Property<string>(lt, "CodPais"),
                                    Pais = lt.Pais.Id,
                                    CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                                    CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                                    CodigoUf = EF.Property<string>(lt, "CodUf"),
                                    Estado = lt.Estado.Id,
                                    Uf = lt.Uf.Id,
                                    Municipio = lt.Municipio,
                                    Regiao = lt.Regiao.Id,
                                })
                                .AsNoTracking()
                                .ToDataTableResponseAsync(request);

            return result == null ?
                NoContent() :
                Ok(result);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<LocalizacaoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<LocalizacaoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType, SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] LocalizacaoRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return Ok(await _context.Localizacoes
                            .AsNoTracking()
                            .Select(lt => new LocalizacaoViewModel
                            {
                                Id = lt.Id,
                                CodigoPais = EF.Property<string>(lt, "CodPais"),
                                Pais = lt.Pais.Id,
                                CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                                CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                                CodigoUf = EF.Property<string>(lt, "CodUf"),
                                Estado = lt.Estado.Id,
                                Uf = lt.Uf.Id,
                                Municipio = lt.Municipio,
                                Regiao = lt.Regiao.Id,
                            })
                            .ToDataTableResponseAsync(request));
            }

            //independentemente dos subníveis dos usuários 'cliente'
            if (request.PorGrupoDoUsuario)
            {
                var queryBuilderLocalizacoes = new StringBuilder(@"
                    select l.*
                    from localizacao l 
                    where exists (select 1 from usuario_adm uat
                                  inner join cliente_unidades cut on cut.localizacao_id_localizacao = l.id_localizacao
                                  where uat.email_usuario = @email
                                  and case when uat.nivel = 'Ineditta' then true 
                                  else cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon end)
                ");

                var queryLocalizacoes = _context.Localizacoes
                    .FromSqlRaw(queryBuilderLocalizacoes.ToString(), new MySqlParameter("@email", _userInfoService.GetEmail()))
                    .Select(lt => new LocalizacaoViewModel
                    {
                        Id = lt.Id,
                        CodigoPais = EF.Property<string>(lt, "CodPais"),
                        Pais = lt.Pais.Id,
                        CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                        CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                        CodigoUf = EF.Property<string>(lt, "CodUf"),
                        Estado = lt.Estado.Id,
                        Uf = lt.Uf.Id,
                        Municipio = lt.Municipio,
                        Regiao = lt.Regiao.Id
                    });

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await queryLocalizacoes.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await queryLocalizacoes.ToListAsync());
            }

            if (request.PorUsuario)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                StringBuilder stringBuilder;

                if (request.LocalizacoesPorAcompanhamentos)
                {
                    stringBuilder = new StringBuilder(@"
                        SELECT l.* FROM localizacao l 
                        WHERE EXISTS (
	                        SELECT * FROM cliente_unidades cut
	                        INNER JOIN usuario_adm ua ON ua.email_usuario = @email
	                        INNER JOIN acompanhamento_cct_estabelecimento_tb acet ON acet.estabelecimento_id = cut.id_unidade 
	                        INNER JOIN acompanhamento_cct_localizacao_tb aclt ON aclt.acompanhamento_cct_id = acet.acompanhamento_cct_id
	                        WHERE CASE WHEN ua.nivel = 'Ineditta' THEN TRUE 
			                           WHEN ua.nivel = 'Grupo Econômico' THEN cut.cliente_grupo_id_grupo_economico = ua.id_grupoecon 
			                           ELSE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
		                          END
		                          AND cut.localizacao_id_localizacao = l.id_localizacao
		                          AND aclt.localizacao_id = cut.localizacao_id_localizacao
                    ");
                }
                else
                {
                    stringBuilder = new StringBuilder(@$"select l.*
                                    from localizacao l 
                                    where exists (select 1 from usuario_adm uat
                                                  inner join cliente_unidades cut on cut.localizacao_id_localizacao = l.id_localizacao
			                                      where uat.email_usuario = @email
			                                      and case when uat.nivel = @nivel then true
                                                           when uat.nivel = @nivelGrupoEconomico then cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon
                                                           else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end");
                }

                parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                if (request.GrupoEconomicoId > 0)
                {
                    stringBuilder.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                    parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
                }

                if(request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
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

                stringBuilder.Append(')');

                if (request.TipoLocalidade == "uf")
                {
                    var queryUf = _context.Localizacoes
                       .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                       .AsNoTracking()
                       .GroupBy(lt => lt.Uf.Id)
                       .Select(lt => new LocalizacaoViewModel
                       {
                           CodigoUf = EF.Property<string>(lt.First(), "CodUf"),
                           Estado = lt.First().Estado.Id,
                           Uf = lt.First().Uf.Id,
                       });

                    return !string.IsNullOrEmpty(request.Columns)
                       ? Ok(await queryUf.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
                       : Ok(await queryUf.ToListAsync());
                }

                if (request.TipoLocalidade == "regiao")
                {
                    var queryRegiao = _context.Localizacoes
                       .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                       .AsNoTracking()
                       .GroupBy(lt => lt.Regiao.Id)
                       .Select(lt => new LocalizacaoViewModel
                       {
                           CodigoRegiao = EF.Property<string>(lt.First(),"CodRegiao"),
                           Regiao = lt.First().Regiao.Id
                       });

                    return !string.IsNullOrEmpty(request.Columns)
                       ? Ok(await queryRegiao.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
                       : Ok(await queryRegiao.ToListAsync());
                }

                var query = _context.Localizacoes
                    .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(lt => new LocalizacaoViewModel
                    {
                        Id = lt.Id,
                        CodigoPais = EF.Property<string>(lt, "CodPais"),
                        Pais = lt.Pais.Id,
                        CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                        CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                        CodigoUf = EF.Property<string>(lt, "CodUf"),
                        Estado = lt.Estado.Id,
                        Uf = lt.Uf.Id,
                        Municipio = lt.Municipio,
                        Regiao = lt.Regiao.Id
                    });

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }

            return Ok(await _context.Localizacoes
                          .Select(lt => new LocalizacaoViewModel
                          {
                              IdLocalizacao = lt.Id,
                              CodigoPais = EF.Property<string>(lt, "CodPais"),
                              Pais = lt.Pais.Id,
                              CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                              CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                              CodigoUf = EF.Property<string>(lt, "CodUf"),
                              Estado = lt.Estado.Id,
                              Uf = lt.Uf.Id,
                              Municipio = lt.Municipio,
                              Regiao = lt.Regiao.Id
                          })
                          .AsNoTracking()
                          .ToListAsync());
        }

        [HttpGet("regioes")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<LocalizacaoViewModel>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType, SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterRegioesAsync([FromQuery] LocalizacaoRequest request)
        {
            FormattableString sql = $@"select cc.* from localizacao cc 
                            inner join lateral(
                            select cod_uf, max(id_localizacao) maior_id
                            from localizacao
                            group by 1) cct 
                            where cct.maior_id = cc.id_localizacao";

            //independentemente dos subníveis dos usuários 'cliente'
            if (request.PorGrupoDoUsuario)
            {
                var queryBuilderUfs = new StringBuilder(@"
                    select l.*
                    from localizacao l 
                    where exists (select 1 from usuario_adm uat
                                  inner join cliente_unidades cut on cut.localizacao_id_localizacao = l.id_localizacao
                                  where uat.email_usuario = @email
                                  and case when uat.nivel = 'Ineditta' then true 
                                  else cut.cliente_grupo_id_grupo_economico = uat.id_grupoecon end)
                    group by l.uf
                    order by l.uf
                ");

                var queryUfs = _context.Localizacoes
                    .FromSqlRaw(queryBuilderUfs.ToString(), new MySqlParameter("@email", _userInfoService.GetEmail()))
                    .Select(lt => new LocalizacaoViewModel
                    {
                        Id = lt.Id,
                        CodigoPais = EF.Property<string>(lt, "CodPais"),
                        Pais = lt.Pais.Id,
                        CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                        CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                        CodigoUf = EF.Property<string>(lt, "CodUf"),
                        Estado = lt.Estado.Id,
                        Uf = lt.Uf.Id,
                        Municipio = lt.Municipio,
                        Regiao = lt.Regiao.Id
                    });

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await queryUfs.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await queryUfs.ToListAsync());
            }

            if (request.PorUsuario)
            {
                sql = $@"select cc.* from localizacao cc 
                            inner join lateral(
                            select cod_uf, max(id_localizacao) maior_id
                            from localizacao
                            group by 1) cct 
                            where cct.maior_id = cc.id_localizacao
                            and exists (select 1 from usuario_adm uat 
			                                      where uat.email_usuario = {_userInfoService.GetEmail()}
			                                      and case when uat.nivel = {nameof(Nivel.Ineditta)} then true else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end)";
            }

            var query = _context.Localizacoes.FromSqlInterpolated(sql).AsNoTracking().Select(lt => new LocalizacaoViewModel
            {
                Id = lt.Id,
                CodigoPais = EF.Property<string>(lt, "CodPais"),
                Pais = lt.Pais.Id,
                CodigoRegiao = EF.Property<string>(lt, "CodRegiao"),
                CodigoMunicipio = EF.Property<string>(lt, "CodMunicipio"),
                CodigoUf = EF.Property<string>(lt, "CodUf"),
                Estado = lt.Estado.Id,
                Uf = lt.Uf.Id,
                Municipio = lt.Municipio,
                Regiao = lt.Regiao.Id
            });

            return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
        }


        [HttpGet("select")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterSelectTodos()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NoContent();
            }

            FormattableString query = $@"select cc.* from localizacao cc 
                            inner join lateral(
                            select cod_uf, max(id_localizacao) maior_id
                            from localizacao
                            group by 1) cct 
                            where cct.maior_id = cc.id_localizacao";

            return Ok(await (_context.Localizacoes.FromSql(query).Select(lc => new OptionModel<int>
            {
                Id = EF.Property<string>(lc, "CodUf").IsNullOrEmpty() ? 0 : int.Parse(EF.Property<string>(lc, "CodUf"), CultureInfo.InvariantCulture),
                Description = lc.Uf.Id
            }).ToListAsync()));
        }
    }
}
