using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net.Mime;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/grupos-clausulas")]
    [ApiController]
    public class GrupoClausulaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public GrupoClausulaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
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
                ? Ok(await _context.GrupoClausula
                            .AsNoTracking()
                            .Select(gc => new OptionModel<int>
                            {
                                Id = gc.Id,
                                Description = gc.Nome
                            }).ToListAsync())
                    : NoContent();
        }
    }
}

