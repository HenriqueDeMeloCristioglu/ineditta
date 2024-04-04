using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.Application.Clausulas.Clientes.UseCases.Upsert;
using Ineditta.API.ViewModels.ClausulasClientes.ViewModels;
using Ineditta.API.ViewModels.ClausulasClientes.Requests;

namespace Ineditta.API.Controllers.V1.ClausulasControllers
{
    [Route("v{version:apiVersion}/clausulas-clientes")]
    [ApiController]
    public class ClausulasClientesControllers : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly HttpRequestItemService _httpRequestItemService;

        public ClausulasClientesControllers(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, HttpRequestItemService httpRequestItemService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _httpRequestItemService = httpRequestItemService;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ClausulaClienteViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterPorId([FromQuery] ClausulaClienteRequest request)
        {
            if (Request.Headers.Accept != MediaTypeNames.Application.Json)
            {
                return NotAcceptable();
            }

            var result = await (from ccl in _context.ClausulaCliente
                                join us in _context.UsuarioAdms on EF.Property<int>(ccl, "UsuarioInclusaoId") equals us.Id
                                where (request.GrupoEconomicoId > 0 && ccl.GrupoEconomicoId == request.GrupoEconomicoId) && ccl.ClausulaId == request.ClausulaId
                                select new ClausulaClienteViewModel
                                {
                                    NomeUsuario = us.Nome,
                                    Texto = ccl.Texto,
                                    DataInclusao = DateOnly.FromDateTime(EF.Property<DateTime>(ccl, "DataInclusao"))
                                })
                                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }


        [HttpGet("{clausulaId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(ClausulaClientePorIdViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterPorId([FromRoute] int clausulaId)
        {
            if (Request.Headers.Accept != MediaTypeNames.Application.Json)
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return Unauthorized();
            }

            var result = await (from ccl in _context.ClausulaCliente
                                join cg in _context.ClausulaGerals on ccl.ClausulaId equals cg.Id
                                join es in _context.EstruturaClausula on cg.EstruturaClausulaId equals es.Id
                                where ccl.ClausulaId == clausulaId && ccl.GrupoEconomicoId == usuario.GrupoEconomicoId
                                select new ClausulaClientePorIdViewModel
                                {
                                    Id = ccl.Id,
                                    Nome = es.Nome,
                                    Texto = ccl.Texto,
                                    DataInclusao = EF.Property<DateTime>(ccl, "DataInclusao")
                                })
                                .OrderByDescending(c => c.DataInclusao)
                                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> Incluir([FromBody] IncluirClausulaClienteRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }
    }
}
