using System.Net.Mime;
using System.Text;

using Ineditta.API.Factories.Sindicatos;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Sindicatos.Patronais.UseCases.Upsert;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
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

namespace Ineditta.API.Controllers.V1.SindicatosControllers
{
    [Route("v{version:apiVersion}/sindicatos-patronais")]
    [ApiController]
    public class SindicatoPatronalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly SindicatoPatronalFactory _sindicatoPatronalFactory;
        private readonly IUserInfoService _userInfoService;

        public SindicatoPatronalController(IMediator mediator, SindicatoPatronalFactory sindicatoPatronalFactory, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService)
            : base(mediator, requestStateValidator)
        {
            _sindicatoPatronalFactory = sindicatoPatronalFactory;
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertSindicatoPatronalRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertSindicatoPatronalRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<SindicatoPatronalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<SindicatoPatronalViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType, SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] SindicatoPatronalRequest request)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return Ok(await _context.SindPatrs
                            .AsNoTracking()
                            .Select(sdt => new SindicatoPatronalViewModel
                            {
                                Id = sdt.Id,
                                Sigla = sdt.Sigla,
                                Cnpj = sdt.Cnpj.Value ?? string.Empty,
                                RazaoSocial = sdt.RazaoSocial ?? string.Empty,
                                Municipio = sdt.Municipio ?? string.Empty,
                                Uf = sdt.Uf ?? string.Empty,
                                Telefone = sdt.Telefone1.Valor,
                                Email = sdt.Email1 == null ? default : sdt.Email1!.Valor,
                                Site = sdt.Site,
                            })
                            .ToDataTableResponseAsync(request));
            }

            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var query = _context.SindPatrs
                            .AsNoTracking()
                            .Select(sdt => new OptionModel<int>
                            {
                                Id = sdt.Id,
                                Description = sdt.Sigla + " / " + CNPJ.Formatar(sdt.Cnpj.Value != null ? sdt.Cnpj.Value : string.Empty) + " / " + sdt.Denominacao,
                            });

                if (request.PorUsuario)
                {
                    var parameters = new List<MySqlParameter>();
                    int parametersCount = 0;

                    var stringBuilder = new StringBuilder(@"select spt.* from sind_patr spt 
                                    where exists(select 1 from cliente_unidades cut
			                                     inner join usuario_adm uat on uat.email_usuario = @email
			                                     inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
			                                     inner join base_territorialsindpatro btspt on lt.id_localizacao = btspt.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{{""id"":', btspt.classe_cnae_idclasse_cnae, '}}'))
			                                     where case when uat.nivel = @nivel then true 
                                                            when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                            else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
			                                     and spt.id_sindp = btspt.sind_patronal_id_sindp");

                    parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
                    parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
                    parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                    if (request.GrupoEconomicoId > 0)
                    {
                        stringBuilder.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                        parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
                    }

                    if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
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

                    if (request.CnaesIds != null && request.CnaesIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.CnaesIds, "btspt.classe_cnae_idclasse_cnae", parameters, ref parametersCount);
                    }

                    if ((request.LocalizacoesIds != null && request.LocalizacoesIds.Any()) || (request.Ufs != null && request.Ufs.Any()))
                    {
                        if ((request.LocalizacoesIds != null && request.LocalizacoesIds.Any()) && (request.Ufs != null && request.Ufs.Any()))
                        {
                            QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.LocalizacoesIds, "lt.id_localizacao", request.Ufs, "lt.uf", parameters, ref parametersCount);
                        }
                        else if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.LocalizacoesIds, "lt.id_localizacao", parameters, ref parametersCount);
                        }
                        else if (request.Ufs != null && request.Ufs.Any())
                        {
                            QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.Ufs, "lt.uf", parameters, ref parametersCount);
                        }
                    }

                    if (request.Regioes != null && request.Regioes.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.Regioes, "lt.regiao", parameters, ref parametersCount);
                    }

                    stringBuilder.Append(')');

                    query = _context.SindPatrs
                    .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
                    .AsNoTracking()
                    .Select(sdt => new OptionModel<int>
                    {
                        Id = sdt.Id,
                        Description = sdt.Sigla + " - " + sdt.Denominacao
                    });
                }

                return !string.IsNullOrEmpty(request.Columns)
                      ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
                      : Ok(await query.ToListAsync());
            }

            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = _context.SindPatrs
                            .AsNoTracking()
                            .Select(sdt => new SindicatoPatronalViewModel
                            {
                                Id = sdt.Id,
                                Cnpj = sdt.Cnpj.Value,
                                Sigla = sdt.Sigla,
                                Municipio = sdt.Municipio,
                                Uf = sdt.Uf,
                                Telefone = sdt.Telefone1.Valor,
                                Email = sdt.Email1 == null ? default : sdt.Email1!.Valor,
                                RazaoSocial = sdt.RazaoSocial
                            });


                if (request.PorUsuario)
                {
                    var result = await _sindicatoPatronalFactory.CriarPorUsuario(request);

                    return result is not null ? Ok(result) : NoContent();
                }

                return !string.IsNullOrEmpty(request.Columns)
              ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
              : Ok(await query.ToListAsync());
            }

            return NotAcceptable();

#pragma warning restore CS8601 // Possible null reference assignment.
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(SindicatoPatronalItemViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id, [FromQuery] SindicatoPatronalPorIdRequest request)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            var sindicato = await (from sp in _context.SindPatrs
                                   join conft in _context.Associacoes on sp.ConfederacaoId equals conft.IdAssociacao into _conft
                                   from conft in _conft.DefaultIfEmpty()
                                   join fdrt in _context.Associacoes on sp.FederacaoId equals fdrt.IdAssociacao into _fdrt
                                   from fdrt in _fdrt.DefaultIfEmpty()
                                   where sp.Id == id
                                   select new SindicatoPatronalItemViewModel
                                   {
                                       Id = sp.Id,
                                       Cnpj = sp.Cnpj.Value,
                                       CodigoSindical = sp.CodigoSindical.Valor,
                                       Sigla = sp.Sigla,
                                       Situacao = sp.Situacao,
                                       RazaoSocial = sp.RazaoSocial,
                                       Denominacao = sp.Denominacao,
                                       Logradouro = sp.Logradouro,
                                       Municipio = sp.Municipio,
                                       Uf = sp.Uf,
                                       Telefone1 = sp.Telefone1.Valor,
                                       Telefone2 = sp.Telefone2 == null ? default : sp.Telefone2!.Valor,
                                       Telefone3 = sp.Telefone3 == null ? default : sp.Telefone3!.Valor,
                                       Ramal = sp.Ramal == null ? default : sp.Ramal!.Valor,
                                       Negociador = sp.Negociador,
                                       Contribuicao = sp.Contribuicao,
                                       Enquadramento = sp.Enquadramento,
                                       Email1 = sp.Email1 == null ? default : sp.Email1!.Valor,
                                       Site = sp.Site,
                                       Confederacao = sp.ConfederacaoId == null ? default : new OptionModel<int>
                                       {
                                           Id = sp.ConfederacaoId!.Value,
                                           Description = conft == null ? default : conft.Sigla
                                       },
                                       Status = sp.Status ? "ativo" : "",
                                       Email2 = sp.Email2 == null ? default : sp.Email2!.Valor,
                                       Email3 = sp.Email3 == null ? default : sp.Email3!.Valor,
                                       Twitter = sp.Twitter,
                                       Facebook = sp.Facebook,
                                       Instagram = sp.Instagram,
                                       Grau = sp.Grau.GetDescription(),
                                       Federacao = sp.FederacaoId == null ? default : new OptionModel<int>
                                       {
                                           Id = sp.FederacaoId!.Value,
                                           Description = fdrt == null ? default : fdrt.Sigla
                                       },
                                       RamalSp = sp.Ramal == null ? default : sp.Ramal!.Valor
                                   })
                                    .SingleOrDefaultAsync();
#pragma warning restore S3358 // Ternary operators should not be nested

            if (sindicato == null)
            {
                return NotFound();
            }

            if (request.CnaesIds is not null && request.CnaesIds.Any())
            {
                var resultAtividadesEconomicas = await (from bp in _context.BaseTerritorialsindpatros
                                                        join cn in _context.ClasseCnaes on bp.CnaeId equals cn.Id into _cn
                                                        from cn in _cn.DefaultIfEmpty()
                                                        where bp.SindicatoId == id && request.CnaesIds.Contains(cn.Id)
                                                        select new
                                                        {
                                                            Atividade = cn.DescricaoSubClasse
                                                        }
                                                     ).ToListAsync();

                if (resultAtividadesEconomicas != null && resultAtividadesEconomicas.Count > 0)
                {
                    sindicato.AtividadesEconomicas = resultAtividadesEconomicas.Select(a => a.Atividade);
                }
            }

            var comentarioPatronal = await (from cm in _context.Comentarios
                                           where cm.Tipo == TipoComentario.SindicatoPatronal && (cm.ReferenciaId == sindicato.Id && cm.Tipo == TipoComentario.SindicatoPatronal)
                                           select cm).FirstOrDefaultAsync();

            if (comentarioPatronal != null)
            {
                sindicato.Comentario = comentarioPatronal.Valor;
            }

            return Ok(sindicato);
        }

        [HttpGet]
        [Route("bases-existentes")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(SindicatoPatronalItemViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterComBaseExistenteAsync()
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }


            var result = await (from se in _context.SindPatrs
                                where _context.BaseTerritorialsindpatros.Any(bem => bem.SindicatoId == se.Id)
                                select new SindicatoPatronalBaseExistenteViewModel
                                {
                                    Id = se.Id,
                                    Sigla = se.Sigla,
                                    Denominacao = se.Denominacao,
                                    Cnpj = se.Cnpj.Value
                                })
                        .AsNoTracking()
                        .ToArrayAsync();

            if (result == null) return NoContent();

            return Ok(result);
        }

        [HttpGet]
        [Route("ufs")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<string>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterUfsAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var result = await (from s in _context.SindPatrs
                                select new OptionModel<string>
                                {
                                    Id = s.Uf,
                                    Description = s.Uf
                                })
                        .Distinct()
                        .ToArrayAsync();

            if (result == null) return NoContent();

            return Ok(result);
        }
    }
}
