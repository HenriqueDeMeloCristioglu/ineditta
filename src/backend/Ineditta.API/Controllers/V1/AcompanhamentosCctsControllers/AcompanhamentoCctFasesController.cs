using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1.AcompanhamentosCcts
{
    [Route("v{version:apiVersion}/acompanhamentos-cct-fases")]
    [ApiController]
    public class AcompanhamentoCctFasesController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public AcompanhamentoCctFasesController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(OptionModel<long>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NoContent();
            }

            var result = await (from acct in _context.FasesCct
                                select new OptionModel<long>
                                {
                                    Id = acct.Id,
                                    Description = acct.Fase
                                })
                                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
