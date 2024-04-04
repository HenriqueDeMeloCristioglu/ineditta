using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Ineditta.API.ViewModels.Jornadas.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/jornada")]
    [ApiController]
    public class JornadaController: ApiBaseController
    {

        private readonly InedittaDbContext _context;

        public JornadaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<JornadasViewModel>), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from jr in _context.Jornada
                                    select new JornadasViewModel
                                    {
                                        Id = jr.Id,
                                        Descricao = jr.Descricao,
                                        Jornada = jr.JornadaSemanal
                                    })
                                    .ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }
    }
}
