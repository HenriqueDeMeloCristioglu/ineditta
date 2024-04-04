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
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Comentarios.UseCases.Upsert;
using Ineditta.Application.Comentarios.Entities;
using Ineditta.API.ViewModels.Comentario.Requests;
using Ineditta.API.ViewModels.Comentario.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1.ComentariosControllers
{
    [Route("v{version:apiVersion}/comentarios")]
    [ApiController]
    public class ComentarioController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly HttpRequestItemService _httpRequestItemService;
        public ComentarioController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, HttpRequestItemService httpRequestItemService)
            : base(mediator, requestStateValidator)
        {
            _context = context;
            _httpRequestItemService = httpRequestItemService;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ComentarioItemViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            var result = await (from cm in _context.Comentarios
                                where cm.Id == id
                                join ut in _context.UsuarioAdms on EF.Property<int>(cm, "UsuarioInclusaoId") equals ut.Id into _ut
                                from ut in _ut.DefaultIfEmpty()
                                join eq in _context.Etiquetas on cm.EtiquetaId equals eq.Id
                                join teq in _context.TiposEtiquetas on eq.TipoEtiquetaId equals teq.Id
                                select new ComentarioItemViewModel
                                {
                                    Id = cm.Id,
                                    TipoComentario = new OptionModel<int>
                                    {
                                        Id = (int)cm.Tipo,
                                        Description = cm.Tipo.ToString()
                                    },
                                    Assunto = new OptionModel<int>
                                    {
                                        Id = cm.ReferenciaId,
                                        Description = ""
                                    },
                                    AdministradorId = ut.Id,
                                    AdministradorNome = ut.Nome,
                                    Comentario = cm.Valor,
                                    DataFinal = cm.DataValidade,
                                    TipoUsuarioDestino = cm.TipoUsuarioDestino,
                                    UsuarioDestinoId = cm.UsuarioDestionoId,
                                    UsuarioDestinoDescricao = "",
                                    TipoNotificacaoId = cm.TipoNotificacao,
                                    Etiqueta = new ComentarioEtiqueta
                                    {
                                        Id = eq.Id,
                                        Nome = eq.Nome,
                                        Tipo = new ComentarioTipoEtiqueta
                                        {
                                            Id = teq.Id,
                                            Nome = teq.Nome
                                        }
                                    },
                                    Visivel = cm.Visivel
                                })
                                .SingleOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            switch (result.TipoComentario.Id)
            {
                case (int)TipoComentario.Clausula:
                    result!.Assunto.Description = await _context.EstruturaClausula.Where(ect => ect.Id == result.Assunto.Id).Select(ect => ect.Nome).FirstOrDefaultAsync();
                    break;
                case (int)TipoComentario.SindicatoPatronal:
                    result.Assunto.Description = await _context.SindPatrs.Where(spt => spt.Id == result.Assunto.Id).Select(sdt => sdt.Sigla + " - " + sdt.Denominacao).FirstOrDefaultAsync();
                    break;
                case (int)TipoComentario.SindicatoLaboral:
                    result.Assunto.Description = await _context.SindEmps.Where(set => set.Id == result.Assunto.Id).Select(set => set.Sigla + " - " + set.Denominacao).FirstOrDefaultAsync();
                    break;
                case (int)TipoComentario.Filial:
                    result.Assunto.Description = await _context.ClienteUnidades.Where(cut => cut.Id == result.Assunto.Id).Select(cut => cut.Nome).FirstOrDefaultAsync();
                    break;
                default:
                    break;
            }

            switch (result.UsuarioDestinoId)
            {
                case (int)TipoUsuarioDestino.Grupo:
                    var grupoEconomico = await _context.GrupoEconomico
                        .Where(cgt => cgt.Id == result.UsuarioDestinoId)
                        .Select(cgt => cgt.Nome).FirstOrDefaultAsync();

                    if (grupoEconomico != null)
                    {
                        result.UsuarioDestinoDescricao = grupoEconomico;
                    }

                    break;
                case (int)TipoUsuarioDestino.Matriz:
                    var matriz = await _context.ClienteMatrizes
                        .Where(cmt => cmt.Id == result.UsuarioDestinoId)
                        .Select(cmt => cmt.Nome + " | " + CNPJ.Formatar(EF.Property<string>(cmt, "Cnpj")))
                        .FirstOrDefaultAsync();

                    if (matriz != null)
                    {
                        result.UsuarioDestinoDescricao = matriz;
                    }
                    break;
                case (int)TipoUsuarioDestino.Unidade:
                    var estabelecimento = await _context.ClienteUnidades
                        .Where(cut => cut.Id == result.UsuarioDestinoId)
                        .Select(cut => cut.Nome + " | " + CNPJ.Formatar(cut.Cnpj.Value))
                        .FirstOrDefaultAsync();

                    if (estabelecimento != null)
                    {
                        result.UsuarioDestinoDescricao = estabelecimento;
                    }
                    break;
                default:
                    break;
            }

            return Ok(result);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ComentarioDataTableViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ComentarioDataTableViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] ComentarioRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null) return Unauthorized();

            if (request.PorUsuario == true && usuario.Nivel != Nivel.Ineditta)
            {
                if (usuario.Nivel == Nivel.GrupoEconomico)
                {
#pragma warning disable IDE0075 // Simplify conditional expression
#pragma warning disable S1125 // Boolean literals should not be redundant
                    var resultPorGrupoEconomico = await (from cm in _context.ComentariosVw
                                                         join ut in _context.UsuarioAdms on cm.UsuarioId equals ut.Id into _ut
                                                         from ut in _ut.DefaultIfEmpty()
                                                         join eq in _context.Etiquetas on cm.EtiquetaId equals eq.Id
                                                         join teq in _context.TiposEtiquetas on eq.TipoEtiquetaId equals teq.Id
                                                         where usuario.Id != ut.Id ? cm.Visivel && ut.GrupoEconomicoId == usuario.GrupoEconomicoId
                                                         : true
                                                         select new ComentarioDataTableViewModel
                                                         {
                                                             Id = cm.Id,
                                                             TipoComentario = cm.Tipo,
                                                             TipoUsuarioDestino = cm.TipoUsuarioDestino,
                                                             TipoNotificacao = cm.TipoNotificacao,
                                                             DataFinal = cm.DataValidade,
                                                             UsuarioNome = ut.Nome,
                                                             Comentario = cm.Comentario,
                                                             EtiquetaNome = cm.EtiquetaNome,
                                                             ClausulaNome = cm.NomeClausula ?? string.Empty,
                                                             SindicatoLaboral = cm.SiglaSindicatoLaboral ?? string.Empty,
                                                             SindicatoPatronal = cm.SiglaSindicatoPatronal ?? string.Empty
                                                         })
                                     .ToDataTableResponseAsync(request);
#pragma warning restore S1125 // Boolean literals should not be redundant
#pragma warning restore IDE0075 // Simplify conditional expression

                    if (resultPorGrupoEconomico == null)
                    {
                        return NoContent();
                    }

                    return Ok(resultPorGrupoEconomico);
                }

#pragma warning disable IDE0075 // Simplify conditional expression
#pragma warning disable S1125 // Boolean literals should not be redundant
                var resultPorEstabelecimento = await (from cm in _context.ComentariosVw
                                                      join ut in _context.UsuarioAdms on cm.UsuarioId equals ut.Id into _ut
                                                      from ut in _ut.DefaultIfEmpty()
                                                      join eq in _context.Etiquetas on cm.EtiquetaId equals eq.Id
                                                      join teq in _context.TiposEtiquetas on eq.TipoEtiquetaId equals teq.Id
                                                      where usuario.Id != ut.Id ? cm.Visivel && usuario.EstabelecimentosIds!.Any(id => ut.EstabelecimentosIds!.Contains(id)) : true
                                                      select new ComentarioDataTableViewModel
                                                      {
                                                          Id = cm.Id,
                                                          TipoComentario = cm.Tipo,
                                                          TipoUsuarioDestino = cm.TipoUsuarioDestino,
                                                          TipoNotificacao = cm.TipoNotificacao,
                                                          DataFinal = cm.DataValidade,
                                                          UsuarioNome = ut.Nome,
                                                          Comentario = cm.Comentario,
                                                          EtiquetaNome = cm.EtiquetaNome,
                                                          ClausulaNome = cm.NomeClausula ?? string.Empty,
                                                          SindicatoLaboral = cm.SiglaSindicatoLaboral ?? string.Empty,
                                                          SindicatoPatronal = cm.SiglaSindicatoPatronal ?? string.Empty
                                                      })
                                        .ToDataTableResponseAsync(request);
#pragma warning restore S1125 // Boolean literals should not be redundant
#pragma warning restore IDE0075 // Simplify conditional expression


                if (resultPorEstabelecimento == null)
                {
                    return NoContent();
                }

                return Ok(resultPorEstabelecimento);
            }

            var result = await (from cm in _context.ComentariosVw
                                join ut in _context.UsuarioAdms on cm.UsuarioId equals ut.Id into _ut
                                from ut in _ut.DefaultIfEmpty()
                                join eq in _context.Etiquetas on cm.EtiquetaId equals eq.Id
                                join teq in _context.TiposEtiquetas on eq.TipoEtiquetaId equals teq.Id
                                select new ComentarioDataTableViewModel
                                {
                                    Id = cm.Id,
                                    TipoComentario = cm.Tipo,
                                    TipoUsuarioDestino = cm.TipoUsuarioDestino,
                                    TipoNotificacao = cm.TipoNotificacao,
                                    DataFinal = cm.DataValidade,
                                    UsuarioNome = ut.Nome,
                                    Comentario = cm.Comentario,
                                    EtiquetaNome = cm.EtiquetaNome,
                                    ClausulaNome = cm.NomeClausula ?? string.Empty,
                                    SindicatoLaboral = cm.SiglaSindicatoLaboral ?? string.Empty,
                                    SindicatoPatronal = cm.SiglaSindicatoPatronal ?? string.Empty
                                })
                                .ToDataTableResponseAsync(request);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Incluir([FromBody] UpsertComentarioRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> Atualizar([FromRoute] int id, [FromBody] UpsertComentarioRequest request)
        {
            request.Id = id;

            return await Dispatch(request);
        }
    }
}
