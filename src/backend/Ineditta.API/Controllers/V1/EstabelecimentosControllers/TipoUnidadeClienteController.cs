using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1.EstabelecimentosControllers
{
    [Route("v{version:apiVersion}/tipos-unidades-clientes")]
    [ApiController]
    public class TipoUnidadeClienteController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public TipoUnidadeClienteController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosSelect()
        {
            if (Request.Headers.Accept != SelectMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var result = await (from tuc in _context.TipounidadeClientes
                                   select new OptionModel<int>
                                   {
                                       Id = tuc.IdTiponegocio,
                                       Description = tuc.TipoNegocio
                                   }).ToListAsync();

            if(result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
