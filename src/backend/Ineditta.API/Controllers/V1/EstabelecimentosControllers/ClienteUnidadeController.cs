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
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Auth;
using System.Text;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Extensions;
using Microsoft.IdentityModel.Tokens;
using Ineditta.Application.ClientesUnidades.UseCases;
using Ineditta.API.Services;
using Ineditta.API.Filters;
using MySqlConnector;
using Ineditta.API.ViewModels.Shared.Requests;
using Ineditta.API.ViewModels.ClientesUnidades.Requests;
using Ineditta.API.ViewModels.ClientesUnidades.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.ClientesUnidades.ViewModels.Contadores;
using Ineditta.API.ViewModels.PerfilEstabelecimentos;
using Ineditta.Repository.ClientesUnidades.Views;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ClienteUnidadeController : ApiBaseController
    {
        private readonly HttpRequestItemService _httpRequestItemService;
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public ClienteUnidadeController(HttpRequestItemService httpRequestItemService, IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _httpRequestItemService = httpRequestItemService;
            _userInfoService = userInfoService;
        }

        [HttpGet]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ClienteUnidadeViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<EstabelecimentoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterPorGrupoEconomicoAsync([FromQuery] ClienteUnidadeDatatableRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                return Ok(await _context.ClienteUnidades
                                        .Select(cut => new OptionModel<int>
                                        {
                                            Id = cut.Id,
                                            Description = cut.Nome
                                        })
                                        .ToListAsync());
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var usuario = _httpRequestItemService.ObterUsuario();
                if (usuario == null)
                {
                    return NoContent();
                }
                if (request.GrupoUsuario)
                {
                    if (request.SindicatoPatronalId is not null && request.SindicatoPatronalId > 0)
                    {
                        var parameters = new List<MySqlParameter>();
                        var parametersCount = 0;

                        var queryBuilderPorSindicatoPatronal = new StringBuilder(@"
                            SELECT cut.* FROM cliente_unidades cut
                            INNER JOIN usuario_adm uat ON uat.email_usuario = @usuarioEmail
                            WHERE CASE WHEN uat.nivel = @nivelIneditta then true 
                                       WHEN uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                       ELSE JSON_CONTAINS(uat.ids_fmge, cast(cut.id_unidade as char)) 
                                  END
                                  AND EXISTS (
	                                    SELECT 1 FROM base_territorialsindpatro bt 
	                                    JOIN cnae_emp ce ON cut.id_unidade = ce.cliente_unidades_id_unidade
	                                    WHERE bt.sind_patronal_id_sindp = @sindicatoPatronalId
	                                            AND bt.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
	                                            AND ce.classe_cnae_idclasse_cnae = bt.classe_cnae_idclasse_cnae      
                                   )
                        ");

                        parameters.Add(new MySqlParameter("@sindicatoPatronalId", request.SindicatoPatronalId));
                        parameters.Add(new MySqlParameter("@usuarioEmail", _userInfoService.GetEmail()));
                        parameters.Add(new MySqlParameter("@nivelIneditta", nameof(Nivel.Ineditta)));

                        if (request.BuscarSomentePorIds && request.ClientesUnidadesIds is not null && request.ClientesUnidadesIds.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(queryBuilderPorSindicatoPatronal, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
                        }
                        else if (request.BuscarSomentePorIds)
                        {
                            return NoContent();
                        }
                        else if (request.ApenasAssociados)
                        {
                            queryBuilderPorSindicatoPatronal.Append(@"
                                AND ( EXISTS (
                                    SELECT 1 FROM cliente_unidade_sindicato_patronal_tb cusptb
                                    WHERE cusptb.cliente_unidade_id = cut.id_unidade
                                          AND cusptb.sindicato_patronal_id = @sindicatoPatronalId
                                )
                            ");

                            if (request.ForcarRetornoPorIds && request.ClientesUnidadesIds is not null && request.ClientesUnidadesIds.Any())
                            {
                                queryBuilderPorSindicatoPatronal.Append(@"
                                    OR (TRUE 
                                ");

                                QueryBuilder.AppendListToQueryBuilder(queryBuilderPorSindicatoPatronal, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
                                queryBuilderPorSindicatoPatronal.Append(") ");
                            }

                            queryBuilderPorSindicatoPatronal.Append(") ");
                        }
                            
                        var queryPorSindicatoPatronal = _context.ClienteUnidades
                                                            .FromSqlRaw(queryBuilderPorSindicatoPatronal.ToString(), parameters.ToArray())
                                                            .AsNoTracking()
                                                            .Select(cut => new ClienteUnidadeModalEmpresaViewModel
                                                            {
                                                                Id = cut.Id,
                                                                Nome = cut.Nome,
                                                                NomeMatriz = _context.ClienteMatrizes
                                                                            .Where(cm => cm.Id == cut.EmpresaId)
                                                                            .Select(cm => cm.Nome)
                                                                            .AsSplitQuery()
                                                                            .SingleOrDefault(),

                                                                NomeGrupoEconomico = _context.GrupoEconomico
                                                                            .Where(cg => cg.Id == EF.Property<int>(cut, "GrupoEconomicoId"))
                                                                            .Select(cg => cg.Nome)
                                                                            .AsSplitQuery()
                                                                            .SingleOrDefault(),

                                                                Cnpj = cut.Cnpj.Value,
                                                                CodigoUnidadeCliente = cut.Codigo,
                                                            })
                                                            .ToDataTableResponseAsync(request);

                        var resultQuery = await queryPorSindicatoPatronal;

                        return Ok(resultQuery);
                    }

                    if (usuario.Nivel == Nivel.Ineditta)
                    {
                        return Ok(await (from cut in _context.ClienteUnidades
                                         join ep in _context.ClienteMatrizes on cut.EmpresaId equals ep.Id into _ep
                                         from ep in _ep.DefaultIfEmpty()
                                         join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                         from ge in _ge.DefaultIfEmpty()
                                         select new ClienteUnidadeModalEmpresaViewModel
                                         {
                                             Id = cut.Id,
                                             Nome = cut.Nome,
                                             Filial = ep.Nome,
                                             Grupo = ge.Nome,
                                             Cnpj = cut.Cnpj.Value,
                                             NomeGrupoEconomico = ge.Nome
                                         })
                                    .AsNoTracking()
                                    .ToDataTableResponseAsync(request));
                    }

                    if (usuario.Nivel == Nivel.GrupoEconomico)
                    {
                        return Ok(await (from cut in _context.ClienteUnidades
                                         join ep in _context.ClienteMatrizes on cut.EmpresaId equals ep.Id into _ep
                                         from ep in _ep.DefaultIfEmpty()
                                         join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                         from ge in _ge.DefaultIfEmpty()
                                         where ep.GrupoEconomicoId == usuario.GrupoEconomicoId
                                         select new ClienteUnidadeModalEmpresaViewModel
                                         {
                                             Id = cut.Id,
                                             Nome = cut.Nome,
                                             Filial = ep.Nome,
                                             Grupo = ge.Nome,
                                             Cnpj = cut.Cnpj.Value,
                                             NomeGrupoEconomico = ge.Nome
                                         })
                                    .AsNoTracking()
                                    .ToDataTableResponseAsync(request));
                    }

                    if (usuario.Nivel == Nivel.Matriz || usuario.Nivel == Nivel.Unidade)
                    {
                        return Ok(await (from cut in _context.ClienteUnidades
                                         join ep in _context.ClienteMatrizes on cut.EmpresaId equals ep.Id into _ep
                                         from ep in _ep.DefaultIfEmpty()
                                         join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                         from ge in _ge.DefaultIfEmpty()
                                         where (usuario.EstabelecimentosIds == null || usuario.EstabelecimentosIds.Length == 0 || usuario.EstabelecimentosIds.Contains(cut.Id))
                                         select new ClienteUnidadeModalEmpresaViewModel
                                         {
                                             Id = cut.Id,
                                             Nome = cut.Nome,
                                             Filial = ep.Nome,
                                             Grupo = ge.Nome,
                                             Cnpj = cut.Cnpj.Value,
                                             NomeGrupoEconomico = ge.Nome
                                         })
                                    .AsNoTracking()
                                    .ToDataTableResponseAsync(request));
                    }

                    return Ok(await (from cut in _context.ClienteUnidades
                                     join ep in _context.ClienteMatrizes on cut.EmpresaId equals ep.Id into _ep
                                     from ep in _ep.DefaultIfEmpty()
                                     join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                     from ge in _ge.DefaultIfEmpty()
                                     where ep.GrupoEconomicoId == usuario.GrupoEconomicoId
                                     select new ClienteUnidadeModalEmpresaViewModel
                                     {
                                         Id = cut.Id,
                                         Nome = cut.Nome,
                                         Filial = ep.Nome,
                                         Grupo = ge.Nome,
                                         Cnpj = cut.Cnpj.Value,
                                         NomeGrupoEconomico = ge.Nome
                                     })
                                    .AsNoTracking()
                                    .ToDataTableResponseAsync(request));
                }

                if (request.GrupoEconomicoId != null ||
                    request.GruposEconomicosIds != null)
                {
                    var parameters = new List<MySqlParameter>();
                    int parametersCount = 0;

                    var query = new StringBuilder(@"select vw.* from estabelecimentos_vw vw
		                            where true");

                    if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(query, request.GruposEconomicosIds, "vw.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
                    }

                    if (request.GrupoEconomicoId != null && request.GrupoEconomicoId.HasValue)
                    {
                        query.Append(" and vw.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                        parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
                    }

                    return Ok(await _context.EstabelecimentosVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                        .AsNoTracking()
                        .Select(ev => new ClienteUnidadeModalEmpresaViewModel
                        {
                            Id = ev.Id,
                            Nome = ev.Nome,
                            Filial = ev.Filial,
                            Grupo = ev.Grupo,
                            Cnpj = ev.Cnpj,
                            NomeGrupoEconomico = ev.NomeGrupoEconomico
                        })
                        .ToDataTableResponseAsync(request));
                }

                var result = await (from cu in _context.ClienteUnidades
                                    join lc in _context.Localizacoes on cu.LocalizacaoId equals lc.Id into _lc
                                    from lc in _lc.DefaultIfEmpty()
                                    join tun in _context.TipounidadeClientes on cu.TipoNegocioId equals tun.IdTiponegocio into _tun
                                    from tun in _tun.DefaultIfEmpty()
                                    join ep in _context.ClienteMatrizes on cu.EmpresaId equals ep.Id into _ep
                                    from ep in _ep.DefaultIfEmpty()
                                    join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                    from ge in _ge.DefaultIfEmpty()
                                    select new ClienteUnidadeDataTableDefaultViewModel
                                    {
                                        Id = cu.Id,
                                        Nome = ep.Nome,
                                        Cnpj = cu.Cnpj.Value,
                                        DataInativacao = cu.DataAusencia,
                                        DataAtivacao = EF.Property<DateTime?>(cu, "DataInclusao"),
                                        NomeEstabelecimento = cu.Nome,
                                        Municipio = lc.Municipio,
                                        Uf = lc.Uf.Id,
                                        NomeGrupoEconomico = ge.Nome,
                                        TipoFilial = tun.TipoNegocio,
                                        IdGrupoEconomico = ge.Id,
                                        IdMatriz = ep.Id
                                    })
                            .AsNoTracking()
                            .ToDataTableResponseAsync(request);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = (from cut in _context.ClienteUnidades
                             join ep in _context.ClienteMatrizes on cut.EmpresaId equals ep.Id into _ep
                             from ep in _ep.DefaultIfEmpty()
                             where (!request.GrupoEconomicoId.HasValue || ep.GrupoEconomicoId == request.GrupoEconomicoId)
                             && (request.GruposEconomicosIds == null || request.GruposEconomicosIds.Contains(ep.GrupoEconomicoId))
                             select new EstabelecimentoViewModel
                             {
                                 Id = cut.Id,
                                 Nome = cut.Nome,
                                 Cnpj = cut.Cnpj.Value,
                                 Codigo = cut.Codigo,
                                 CodigoSindical = cut.CodigoSindicatoCliente
                             }).AsNoTracking();

                if (request.PorUsuario)
                {
                    var paramenters = new List<MySqlParameter>();
                    int parametersCount = 0;

                    var queryUnidades = new StringBuilder(@"
                        select * from cliente_unidades cut
                                where exists(select 1 from usuario_adm uat
			                                 where uat.email_usuario = @userEmail
			                                 and case when uat.nivel = @nivelIneditta then true 
                                                      when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                      else JSON_CONTAINS(uat.ids_fmge, cast(cut.id_unidade as char)) end)
                    ");

                    paramenters.Add(new MySqlParameter("@userEmail", _userInfoService.GetEmail()));
                    paramenters.Add(new MySqlParameter("@nivelIneditta", nameof(Nivel.Ineditta)));

                    if (request.MatrizesIds is not null && request.MatrizesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(queryUnidades, request.MatrizesIds, "cut.cliente_matriz_id_empresa", paramenters, ref parametersCount);
                    }

                    if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(queryUnidades, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", paramenters, ref parametersCount);
                    }

                    if (request.MatrizId is not null)
                    {
                        queryUnidades.Append(@"and @matrizId = 0 or cut.cliente_matriz_id_empresa = @matrizId");
                        paramenters.Add(new MySqlParameter("@matrizId", request.MatrizId));
                    }

                    query = _context.ClienteUnidades
                        .FromSqlRaw(queryUnidades.ToString(), paramenters.ToArray())
                        .AsNoTracking()
                        .Select(cut => new EstabelecimentoViewModel
                        {
                            Id = cut.Id,
                            Nome = cut.Nome,
                            Cnpj = cut.Cnpj.Value,
                            Codigo = cut.Codigo,
                            CodigoSindical = cut.CodigoSindicatoCliente
                        });
                }

                return !string.IsNullOrEmpty(request.Columns)
              ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
              : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ClienteUnidadeViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterPorId([FromRoute] int id)
        {
            var result = await (from cu in _context.ClienteUnidades
                                join lc in _context.Localizacoes on cu.LocalizacaoId equals lc.Id into _lc
                                from lc in _lc.DefaultIfEmpty()
                                join tuc in _context.TipounidadeClientes on cu.TipoNegocioId equals tuc.IdTiponegocio into _tuc
                                from tuc in _tuc.DefaultIfEmpty()
                                join ep in _context.ClienteMatrizes on cu.EmpresaId equals ep.Id into _ep
                                from ep in _ep.DefaultIfEmpty()
                                join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                from ge in _ge.DefaultIfEmpty()
                                where cu.Id == id
                                select new ClienteUnidadeViewModel
                                {
                                    Id = cu.Id,
                                    Cnpj = cu.Cnpj.Value,
                                    Nome = cu.Nome,
                                    DataAtivacao = EF.Property<DateTime?>(cu, "DataInclusao"),
                                    DataInativacao = cu.DataAusencia,
                                    Uf = lc.Uf.Id,
                                    Municipio = lc.Municipio,
                                    TipoFilial = tuc.TipoNegocio,
                                    NomeGrupoEconomico = ge.Nome,
                                    MatrizId = ep.Id,
                                    CnaeFilialId = cu.CnaeFilial,
                                    CnaesUnidade = cu.CnaesUnidades,
                                    Cep = cu.Logradouro != null ? cu.Logradouro!.Cep.Value : string.Empty,
                                    Bairro = cu.Logradouro != null ? cu.Logradouro!.Bairro : string.Empty,
                                    Regiao = cu.Logradouro != null ? cu.Logradouro!.Regiao : string.Empty,
                                    Logradouro = cu.Logradouro != null ? cu.Logradouro!.Endereco : string.Empty,
                                    LocalizacaoId = lc.Id,
                                    TipoNegocioId = cu.TipoNegocioId,
                                    Codigo = cu.Codigo,
                                    CodigoSindicatoCliente = cu.CodigoSindicatoCliente,
                                    CodigoSindicatoPatronal = cu.CodigoSindicatoPatronal
                                })
                                .AsNoTracking()
                                .SingleAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        [Route("estabelecimentos")]
        public async ValueTask<IActionResult> ObterEstabelecimentos([FromQuery] ClienteUnidadeEstabelecimentoRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            if (request.PorUsuario != null && request.PorUsuario == true)
            {
                var usuario = _httpRequestItemService.ObterUsuario();

                if (usuario == null)
                {
                    return NoContent();
                }

                var resultPorUsuario = await (from cut in _context.ClienteUnidades
                                    join cmt in _context.ClienteMatrizes on cut.EmpresaId equals cmt.Id
                                    join tuct in _context.TipounidadeClientes on cut.TipoNegocioId equals tuct.IdTiponegocio
                                    where EF.Property<int?>(cut, "GrupoEconomicoId") == usuario.GrupoEconomicoId
                                    select new OptionModel<int>
                                    {
                                        Id = cut.Id,
                                        Description = cut.Nome + " | " + CNPJ.Formatar(cut.Cnpj.Value)
                                    })
                                    .AsNoTracking()
                                    .ToListAsync();

                if (resultPorUsuario == null)
                {
                    return NoContent();
                }

                return Ok(resultPorUsuario);
            }

            var result = await (from cut in _context.ClienteUnidades
                                join cmt in _context.ClienteMatrizes on cut.EmpresaId equals cmt.Id
                                join tuct in _context.TipounidadeClientes on cut.TipoNegocioId equals tuct.IdTiponegocio
                                select new
                                {
                                    Id = cut.Id,
                                    cut.Nome,
                                    cut.Cnpj
                                })
                                .AsNoTracking()
                                .ToListAsync();

            return !(result?.Any() ?? false)
                ? NoContent()
                : Ok(result.Select(cut => new OptionModel<int> { Id = cut.Id, Description = cut.Nome + " | " + CNPJ.Formatar(cut.Cnpj.Value) }));
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        [Route("estabelecimentos-por-empresas")]
        public async ValueTask<IActionResult> ObterEstabelecimentosPorEmpresas([FromQuery] IEnumerable<int>? EmpresasIds)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return NoContent();
            }

            var listaEmpresas = new List<int>();

            if (EmpresasIds != null)
            {
                listaEmpresas = new List<int>(EmpresasIds);
            }

            if (usuario.Nivel != Nivel.Ineditta)
            {
                var resultPorUsuario = await (from cut in _context.ClienteUnidades
                                    join cmt in _context.ClienteMatrizes on cut.EmpresaId equals cmt.Id
                                    join tuct in _context.TipounidadeClientes on cut.TipoNegocioId equals tuct.IdTiponegocio
                                    where listaEmpresas.IsNullOrEmpty() || listaEmpresas.Any(x => x == cut.EmpresaId)
                                    where EF.Property<int?>(cut, "GrupoEconomicoId") == usuario!.GrupoEconomicoId
                                    select new
                                    {   
                                        Id = cut.Id,
                                        cut.Nome,   
                                        cut.Cnpj
                                    }).AsNoTracking().ToListAsync();

                return !(resultPorUsuario?.Any() ?? false)
                    ? NoContent()
                    : Ok(resultPorUsuario.Select(cut => new OptionModel<int> { Id = cut.Id, Description = cut.Nome + " | " + CNPJ.Formatar(cut.Cnpj.Value) }));
            }

            var result = await (from cut in _context.ClienteUnidades
                                join cmt in _context.ClienteMatrizes on cut.EmpresaId equals cmt.Id
                                join tuct in _context.TipounidadeClientes on cut.TipoNegocioId equals tuct.IdTiponegocio
                                where listaEmpresas.IsNullOrEmpty() || listaEmpresas.Any(x => x == cut.EmpresaId)
                                select new
                                {
                                    Id = cut.Id,
                                    cut.Nome,
                                    cut.Cnpj
                                }).AsNoTracking().ToListAsync();

            return !(result?.Any() ?? false)
                ? NoContent()
                : Ok(result.Select(cut => new OptionModel<int> { Id = cut.Id, Description = cut.Nome + " | " + CNPJ.Formatar(cut.Cnpj.Value) }));
        }

        [HttpGet("contadores")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ContadorViewModel), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterContadoresAsync([FromQuery] FiltroHomeRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var parameters = new Dictionary<string, object>();

            var sql = new StringBuilder(@"SELECT count(distinct cmt.id_empresa) as empresas,
				                        COUNT(distinct cut.id_unidade) as estabelecimentos,
				                        COUNT(distinct cet.classe_cnae_idclasse_cnae) as cnaes,
				                        COUNT(distinct se.id_sinde) as sindicatos_laborais,
				                        COUNT(distinct sp.id_sindp) as sindicatos_patronais
				                        from cliente_matriz as cmt 
                                        inner join usuario_adm uat on uat.email_usuario = @email
				                        LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
				                        LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
				                        LEFT join base_territorialsindemp AS bl 
					                        on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                        AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
				                        LEFT Join sind_emp as se on se.id_sinde = bl.sind_empregados_id_sinde1
				                        LEFT join base_territorialsindpatro AS bp 
					                        on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					                        AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao) 
					                        LEFT Join sind_patr as sp on sp.id_sindp = bp.sind_patronal_id_sindp
                                        where case when uat.nivel = @nivel then true
                                                   when uat.nivel = @nivelGrupoEconomico then cmt.cliente_grupo_id_grupo_economico = uat.id_grupoecon
                                                   else JSON_CONTAINS(uat.ids_fmge, CONCAT('',cut.id_unidade, '')) end");

            parameters.Add("email", _userInfoService.GetEmail()!);
            parameters.Add("nivel", nameof(Nivel.Ineditta).ToString());
            parameters.Add("nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()!);
            parameters.Add("nivelMatriz", Nivel.Matriz.GetDescription()!);

            if (request.UnidadesIds != null && request.UnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.UnidadesIds, "cut.id_unidade", parameters);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.MatrizesIds, new List<string> { "cut.cliente_matriz_id_empresa", "cmt.id_empresa" }, parameters);
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.GruposEconomicosIds, new List<string> { "cut.cliente_grupo_id_grupo_economico", "cmt.cliente_grupo_id_grupo_economico" }, parameters);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(sql, request.CnaesIds, "cet.classe_cnae_idclasse_cnae", parameters);
            }

            var query = _context.SelectFromRawSqlAsync<ContadorViewModel>(sql.ToString(), parameters);
            var result = (await query).SingleOrDefault();

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertClienteUnidadeRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [Route("{id:int}")]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertClienteUnidadeRequest request)
        {
            if (request is null)
            {
                return EmptyRequestBody();
            }

            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [Route("info-consultor")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<InfoConsultorViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterInformacoesConsultorAsync([FromQuery] ClienteUnidadeDatatableRequest request)
        {
            var parameters = new Dictionary<string, object>
            {
                { "email", _userInfoService.GetEmail()! }
            };

            var query = new StringBuilder(@"select cstt.nome_usuario nome, cstt.telefone, cstt.ramal
                                                from usuario_adm cstt
                                                left join cliente_unidades csut on json_contains(cstt.ids_fmge, cast(csut.id_unidade as json))
                                                left join cliente_matriz cmt on cmt.id_empresa = csut.cliente_matriz_id_empresa
                                                left join cliente_grupo cg on csut.cliente_grupo_id_grupo_economico = cg.id_grupo_economico
                                                where tipo = 'Ineditta'
                                                and ids_fmge != cast('[]' as json)
                                                and ids_fmge != cast('""""' as json)
                                                and exists(
                                                select 1
                                                from cliente_unidades cut
                                                join usuario_adm uat on uat.email_usuario = @email
                                                where case when uat.ids_fmge = cast('[]' as json) or cast('""""' as json) 
                                                        then uat.id_grupoecon = cg.id_grupo_economico 
                                                        else 
                                                             json_contains(uat.ids_fmge, cast(csut.id_unidade as json)) 
                                                        end
                                                        and json_contains(cstt.ids_fmge, cast(cut.id_unidade as json)))
                                                group by cstt.id_user");

            return Ok(await _context.SelectFromRawSqlAsync<InfoConsultorViewModel>(query.ToString(), parameters));
        }

        [HttpPost("obter-por-lista-ids")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ClienteUnidadeDataTableDefaultViewModel), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterPorListaIdsAsync([FromBody] ClienteUnidadePorListaIdsRequest request)
        {
            if (request.ClienteUnidadeIds is null || !request.ClienteUnidadeIds.Any()) return NoContent();

            var listaUnidadesIds = request.ClienteUnidadeIds.ToList();
            var result = await (from cu in _context.ClienteUnidades
                                join lc in _context.Localizacoes on cu.LocalizacaoId equals lc.Id into _lc
                                from lc in _lc.DefaultIfEmpty()
                                join tun in _context.TipounidadeClientes on cu.TipoNegocioId equals tun.IdTiponegocio into _tun
                                from tun in _tun.DefaultIfEmpty()
                                join ep in _context.ClienteMatrizes on cu.EmpresaId equals ep.Id into _ep
                                from ep in _ep.DefaultIfEmpty()
                                join ge in _context.GrupoEconomico on ep.GrupoEconomicoId equals ge.Id into _ge
                                from ge in _ge.DefaultIfEmpty()
                                where listaUnidadesIds.Contains(cu.Id)
                                select new ClienteUnidadeDataTableDefaultViewModel
                                {
                                    Id = cu.Id,
                                    Nome = ep.Nome,
                                    Cnpj = cu.Cnpj.Value,
                                    DataInativacao = cu.DataAusencia,
                                    DataAtivacao = EF.Property<DateTime?>(cu, "DataInclusao"),
                                    NomeEstabelecimento = cu.Nome,
                                    Municipio = lc.Municipio,
                                    Uf = lc.Uf.Id,
                                    NomeGrupoEconomico = ge.Nome,
                                    TipoFilial = tun.TipoNegocio,
                                    IdGrupoEconomico = ge.Id,
                                    IdMatriz = ep.Id
                                })
                            .AsNoTracking()
                            .ToDataTableResponseAsync(request);

            return result == null ?
                NoContent() :
                Ok(result);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<int>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        [Route("estabelecimentos-selecionados")]
        public async ValueTask<IActionResult> FiltrarEstabelecimentosRelacionadosGrupoAsync([FromBody] FiltrarEstabelecimentosSelecionadosRequest request)
        {
            var usuario = _httpRequestItemService.ObterUsuario();
            if (usuario == null)
            {
                return NoContent();
            }

            if (request.GruposIds != null && request.EstabelecimentoIds != null)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"select vw.* from estabelecimentos_vw vw
		                            where true"
                );

                if (request.GruposIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.GruposIds, "vw.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
                }

                var result = await _context.EstabelecimentosVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(ev => new 
                    {
                        ev.Id,
                    })
                    .Where(x => request.EstabelecimentoIds.Contains(x.Id)).ToListAsync();

                return Ok(result);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("informacoes-estabelecimentos")]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<InformacoesEstabelecimentosViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterInformacoesEstabelecimentos([FromQuery] InformacoesEstabelecimentosRequest request)
        {
            var usuario = _httpRequestItemService.ObterUsuario();
            if (usuario == null) return BadRequestFromErrorMessage("Usuário logado não identificado");

            var emailUser = _userInfoService.GetEmail();
            var unidadesIds = request.UnidadesIds ?? new List<int>();

            var informacoes = await _context.InformacoesEstabelecimentosVw
                .Where(i => 
                    (i.EmailUsuario == emailUser) && 
                    (unidadesIds.Any(uId => uId == i.UnidadeId) || !unidadesIds.Any())
                 )
                .Select(i => new InformacoesEstabelecimentosVw
                {
                    SindicatosLaborais = i.SindicatosLaborais != null ? i.SindicatosLaborais.Distinct() : null,
                    SindicatosPatronais = i.SindicatosPatronais != null ? i.SindicatosPatronais.Distinct() : null,
                    SindicatosLaboraisSiglas = i.SindicatosLaboraisSiglas,
                    SindicatosPatronaisSiglas = i.SindicatosPatronaisSiglas,
                    CnpjEstabalecimento = i.CnpjEstabalecimento,
                    CodigoEstabelecimento = i.CodigoEstabelecimento,
                    CodigoSindicatoCliente = i.CodigoSindicatoCliente,
                    DatasBases = i.DatasBases,
                    EmailUsuario = i.EmailUsuario,
                    NomeEstabelecimento = i.NomeEstabelecimento,
                    UnidadeId = i.UnidadeId,
                })
                .ToDataTableResponseAsync(request);

            return Ok(informacoes);
        }
    }
}