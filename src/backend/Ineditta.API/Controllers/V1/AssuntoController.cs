using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Ineditta.API.ViewModels.Shared.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/assuntos")]
    [ApiController]
    public class AssuntoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public AssuntoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync()
        {

            return Request.Headers.Accept == SelectMediaType.ContentType
                ? Ok(await _context.Assuntos
                            .AsNoTracking()
                            .Select(ass => new OptionModel<int>
                            {
                                Id = ass.Idassunto,
                                Description = ass.Assunto1
                            }).ToListAsync())
                    : NoContent();
        }
    }
}
