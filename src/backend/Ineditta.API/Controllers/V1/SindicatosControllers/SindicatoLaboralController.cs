using System.Net.Mime;
using System.Text;

using Ineditta.API.Factories.Sindicatos;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.API.ViewModels.SindicatosLaborais.Requests;
using Ineditta.API.ViewModels.SindicatosLaborais.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Sindicatos.Laborais.UseCases.Upsert;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
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

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/sindicatos-laborais")]
    [ApiController]
    public class SindicatoLaboralController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly SindicatoLaboralFactory _sindicatoLaboralFactory;

        public SindicatoLaboralController(IMediator mediator, SindicatoLaboralFactory sindicatoLaboralFactory, RequestStateValidator requestStateValidator, InedittaDbContext context)
            : base(mediator, requestStateValidator)
        {
            _context = context;
            _sindicatoLaboralFactory = sindicatoLaboralFactory;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> IncluirAsync([FromBody] UpsertSindicatoLaboralRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] UpsertSindicatoLaboralRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<SindicatoLaboralViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<SindicatoLaboralViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType, SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] SindicatoLaboralRequest request)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                return Ok(await _context.SindEmps
                           .AsNoTracking()
                           .Select(sdl => new SindicatoLaboralViewModel
                           {
                               Id = sdl.Id,
                               Cnpj = sdl.Cnpj.Value,
                               Sigla = sdl.Sigla,
                               Logradouro = sdl.Logradouro,
                               Municipio = sdl.Municipio,
                               Uf = sdl.Uf,
                               Telefone = sdl.Telefone1.Valor,
                               Email = sdl.Email1 == null ? default : sdl.Email1!.Valor,
                               Site = sdl.Site,
                               Codigo = sdl.CodigoSindical.Valor,
                               RazaoSocial = sdl.RazaoSocial
                           })
                           .ToDataTableResponseAsync(request));
            }

            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var sindEmps = await _context.SindEmps.AsNoTracking().ToListAsync();
                var formattedSindEmps = sindEmps.Select(sdt => new OptionModel<int>
                {
                    Id = sdt.Id,
                    Description = sdt.Sigla + " / " + CNPJ.Formatar(sdt.Cnpj.Value) + " / " + sdt.Denominacao
                }).ToList();

                return Ok(formattedSindEmps);
            }

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var query = _context.SindEmps
                                .AsNoTracking()
                                .Select(sdl => new SindicatoLaboralViewModel
                                {
                                    Id = sdl.Id,
                                    Cnpj = sdl.Cnpj.Value,
                                    RazaoSocial = sdl.RazaoSocial
                                });

            if (request.PorUsuario)
            {
                var result = await _sindicatoLaboralFactory.CriarPorUsuario(request);

                return result is not null ? Ok(result) : NoContent();
            }

            return !string.IsNullOrEmpty(request.Columns)
            ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
            : Ok(await query.ToListAsync());
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        [HttpGet]
        [Route("filtros-sindicatos")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterFiltrosSindicatosAsync([FromQuery] int? idGrupoEconomico, [FromQuery] IEnumerable<int?> idMatriz, [FromQuery] IEnumerable<int?> idUnidade, [FromQuery] IEnumerable<string?> localizacoes, [FromQuery] string? tipoLocalidade, [FromQuery] IEnumerable<int?> idCnaes)
        {
            IEnumerable<int> localizacoesResult;

            if (tipoLocalidade != null)
            {
                IQueryable<int> query = Enumerable.Empty<int>().AsQueryable();
                if (tipoLocalidade == "uf")
                {
                    query = from loc in _context.Localizacoes
                            .Where(x => localizacoes.Contains(x.Uf.Id))
                            select loc.Id;
                }

                if (tipoLocalidade == "municipio")
                {
                    query = from loc in _context.Localizacoes
                            .Where(x => localizacoes.Contains(x.Municipio))
                            select loc.Id;
                }

                if (tipoLocalidade == "regiao")
                {
                    query = from loc in _context.Localizacoes
                            .Where(x => localizacoes.Contains(x.Regiao.Id))
                            select loc.Id;
                }

                localizacoesResult = await query
                            .GroupBy(id => id)
                            .Select(group => group.Key)
                            .ToListAsync();
            }
            else
            {
                localizacoesResult = Enumerable.Empty<int>();
            }

            var result = await (from sinde in _context.SindEmps
                                join btemp in _context.BaseTerritorialsindemps on sinde.Id equals btemp.SindicatoId into _btemp
                                from btemp in _btemp.DefaultIfEmpty()
                                join loc in _context.Localizacoes on btemp.LocalizacaoId equals loc.Id into _loc
                                from loc in _loc.DefaultIfEmpty()
                                join cu in _context.ClienteUnidades on loc.Id equals cu.LocalizacaoId into _cu
                                from cu in _cu.DefaultIfEmpty()
                                join cm in _context.ClienteMatrizes on cu.EmpresaId equals cm.Id into _cm
                                from cm in _cm.DefaultIfEmpty()
                                join gp in _context.GrupoEconomico on cm.GrupoEconomicoId equals gp.Id into _gp
                                from gp in _gp.DefaultIfEmpty()
                                from ccnae in _context.ClasseCnaes
                                .Where(x => EF.Functions.JsonContains(cu.CnaesUnidades, x.Id))
                                .DefaultIfEmpty()
                                where (idGrupoEconomico == null || gp.Id == idGrupoEconomico) && (!idUnidade.Any() || idUnidade.Any(x => x == cu.Id))
                                && (!idMatriz.Any() || idMatriz.Any(x => x == cm.Id)) && (!localizacoesResult.Any() || localizacoesResult.Any(x => x == loc.Id)
                                && (!idCnaes.Any() || idCnaes.Any(x => x == ccnae.Id)))
                                select new OptionModel<int>
                                {
                                    Id = sinde.Id,
                                    Description = $"{sinde.Sigla} / {sinde.Cnpj.Value} / ${sinde.RazaoSocial}",
                                })
                                .ToListAsync();


            return Request.Headers.Accept == SelectMediaType.ContentType
                ? Ok(result)
                : NoContent();
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(SindicatoLaboralItemViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id, [FromQuery] SindicatoLaboralPorIdRequest request)
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            var result = await (from sl in _context.SindEmps
                                   join conft in _context.Associacoes on sl.ConfederacaoId equals conft.IdAssociacao into _conft
                                   from conft in _conft.DefaultIfEmpty()
                                   join fdrt in _context.Associacoes on sl.FederacaoId equals fdrt.IdAssociacao into _fdrt
                                   from fdrt in _fdrt.DefaultIfEmpty()
                                   join cs in _context.CentralSindicals on sl.CentralSindicalId equals cs.IdCentralsindical into _cs
                                   from cs in _cs.DefaultIfEmpty()
                                   where sl.Id == id
                                   select new SindicatoLaboralItemViewModel
                                   {
                                       Id = sl.Id,
                                       Cnpj = sl.Cnpj.Value,
                                       CodigoSindical = sl.CodigoSindical.Valor,
                                       Sigla = sl.Sigla,
                                       Situacao = sl.Situacao,
                                       RazaoSocial = sl.RazaoSocial,
                                       Denominacao = sl.Denominacao,
                                       Logradouro = sl.Logradouro,
                                       Municipio = sl.Municipio,
                                       Uf = sl.Uf,
                                       Telefone1 = sl.Telefone1 == null ? default : sl.Telefone1!.Valor,
                                       Telefone2 = sl.Telefone2 == null ? default : sl.Telefone2!.Valor,
                                       Telefone3 = sl.Telefone3 == null ? default : sl.Telefone3!.Valor,
                                       Ramal = sl.Ramal == null ? default : sl.Ramal!.Valor,
                                       Negociador = sl.Negociador,
                                       Contribuicao = sl.Contribuicao,
                                       Enquadramento = sl.Enquadramento,
                                       Email1 = sl.Email1 == null ? default : sl.Email1!.Valor,
                                       Site = sl.Site,
                                       Confederacao = sl.ConfederacaoId == null ? default : new OptionModel<int>
                                       {
                                           Id = sl.ConfederacaoId!.Value,
                                           Description = conft == null ? default : conft.Sigla
                                       },
                                       Status = sl.Status ? "ativo" : "inativo",
                                       Email2 = sl.Email2 == null ? default : sl.Email2!.Valor,
                                       Email3 = sl.Email3 == null ? default : sl.Email3!.Valor,
                                       Twitter = sl.Twitter,
                                       Facebook = sl.Facebook,
                                       Instagram = sl.Instagram,
                                       Grau = sl.Grau.ToString(),
                                       Federacao = sl.FederacaoId == null ? default : new OptionModel<int>
                                       {
                                           Id = sl.FederacaoId!.Value,
                                           Description = fdrt == null ? default : fdrt.Sigla
                                       },
                                       CentralSindical = sl.CentralSindicalId == null ? default : new OptionModel<int>
                                       {
                                           Id = sl.CentralSindicalId!.Value,
                                           Description = cs == null ? default : cs.Sigla
                                       }
                                   }).SingleOrDefaultAsync();
#pragma warning restore S3358 // Ternary operators should not be nested

            if (result == null)
            {
                return NotFound();
            }

            if (request.CnaesIds is not null && request.CnaesIds.Any())
            {
                var resultAtividadesEconomicas = await (from be in _context.BaseTerritorialsindemps
                                                        join cn in _context.ClasseCnaes on be.CnaeId equals cn.Id into _cn
                                                        from cn in _cn.DefaultIfEmpty()
                                                        where be.SindicatoId == id && request.CnaesIds.Contains(cn.Id)
                                                        select new
                                                        {
                                                            Atividade = cn.DescricaoSubClasse
                                                        }
                                                     ).ToListAsync();

                if (resultAtividadesEconomicas != null && resultAtividadesEconomicas.Count > 0)
                {
                    result.AtividadesEconomicas = resultAtividadesEconomicas.Select(a => a.Atividade);
                }
            }

            var comentarioLaboral = await (from cm in _context.Comentarios
                                           where cm.Tipo == TipoComentario.SindicatoLaboral && (cm.ReferenciaId == result.Id && cm.Tipo == TipoComentario.SindicatoLaboral)
                                           select cm).FirstOrDefaultAsync();

            if (comentarioLaboral != null)
            {
                result.Comentario = comentarioLaboral.Valor;
            }

            return Ok(result);
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


            var result = await (from se in _context.SindEmps
                        where _context.BaseTerritorialsindemps.Any(bem => bem.SindicatoId == se.Id)
                        select new SindicatoLaboralBaseExistenteViewModel
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

            var result = await (from s in _context.SindEmps
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
