using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.Application.Etiquetas.UseCases.Upsert;
using Ineditta.API.ViewModels.Etiquetas.Requests;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1.ComentariosControllers
{
    [Route("v{version:apiVersion}/etiquetas")]
    [ApiController]
    public class EtiquetaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public EtiquetaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(OptionModel<int>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] EtiquetasObterTotosRequest request)
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var result = await _context.Etiquetas
                .Where(e => request.TipoEtiquetaId != null && e.TipoEtiquetaId == request.TipoEtiquetaId)
                .Select(t => new OptionModel<long>
                {
                    Id = t.Id,
                    Description = t.Nome
                })
                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> Incluir()
        {
            var request = new UpsertEtiquetaRequest { Id = 0 };
            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> Atualizar([FromRoute] int id)
        {
            var request = new UpsertEtiquetaRequest { Id = id };
            return await Dispatch(request);
        }
    }
}
