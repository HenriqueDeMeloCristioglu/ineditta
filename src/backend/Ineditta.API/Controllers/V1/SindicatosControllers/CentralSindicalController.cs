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
using Ineditta.API.ViewModels.CentraisSindicais.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/centrais-sindicais")]
    [ApiController]
    public class CentralSindicalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public CentralSindicalController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<CentralSindicalViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            return Request.Headers.Accept == DatatableMediaType.ContentType
                ? Ok(await _context.CentralSindicals
                            .AsNoTracking()
                            .Select(csd => new CentralSindicalViewModel
                            {
                                Id = csd.IdCentralsindical,
                                Cnpj = csd.Cnpj,
                                Sigla = csd.Sigla,
                                Nome = csd.NomeCentralsindical
                            })
                            .ToDataTableResponseAsync(request))
                : NoContent();
        }
    }
}
