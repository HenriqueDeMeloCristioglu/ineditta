using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1.AcompanhamentosCctsControllers
{
    [Route("v{version:apiVersion}/acompanhamentos-cct-localizacoes")]
    [ApiController]
    public class AcompanhamentoCctLocalizacaoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public AcompanhamentoCctLocalizacaoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ufs")]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(OptionModel<string>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NoContent();
            }

            var result = await (from acct in _context.AcompanhamentoCctLocalizacao
                                join l in _context.Localizacoes on acct.LocalizacaoId equals l.Id
                                select new OptionModel<string>
                                {
                                    Id = l.Uf.Id,
                                    Description = l.Uf.Id
                                })
                                .Distinct()
                                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
