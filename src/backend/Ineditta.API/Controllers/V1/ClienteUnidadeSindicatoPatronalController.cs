using System.Net.Mime;

using Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Atualizar;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/clientes-unidades-sindicatos-patronais")]
    [ApiController]
    public class ClienteUnidadeSindicatoPatronalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public ClienteUnidadeSindicatoPatronalController(InedittaDbContext inedittaDbContext, IMediator mediator, RequestStateValidator requestStateValidator) : base(mediator, requestStateValidator)
        {
            _context = inedittaDbContext;
        }

        [HttpGet]
        [Route("sindicatos/{id:int}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status204NoContent, typeof(Envelope), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorSindicatoId([FromRoute] int id)
        {
            if (Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return Ok(
                    await _context.ClientesUnidadesSindicatosPatronais
                    .Where(cusp => cusp.SindicatoPatronalId == id)
                    .ToListAsync()
                );
            }
            return NoContent();
        }

        [HttpPut]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseMimeType(StatusCodes.Status204NoContent, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> Atualizar(AtualizarClienteUnidadeSindicatoPatronalRequest request)
        {
            return await Dispatch(request);
        }
    }
}
