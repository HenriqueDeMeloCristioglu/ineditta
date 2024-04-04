using System.Globalization;
using System.Net.Mime;

using Ineditta.API.ViewModels.GestaoDeChamados.ViewModels;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/gestoes-chamados")]
    [ApiController]
    public class GestaoDeChamadosController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public GestaoDeChamadosController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<GestaoDeChamadosViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                var result = await (from gdc in _context.Helpdesks
                                    join usslc in _context.UsuarioAdms on gdc.IdUserC equals usslc.Id into _usslc
                                    from usslc in _usslc.DefaultIfEmpty()
                                    join usrsp in _context.UsuarioAdms on (long?) gdc.IdUserR equals usrsp.Id into _usrsp
                                    from usrsp in _usrsp.DefaultIfEmpty()
                                    join cl in _context.ClienteUnidades on (long?)gdc.Estabelecimento equals cl.Id into _cl
                                    from cl in _cl.DefaultIfEmpty()
                                    join md in _context.Modulos on gdc.TipoChamado equals Convert.ToString(md.Id) into _md
                                    from md in _md.DefaultIfEmpty()
                                    select new GestaoDeChamadosViewModel
                                    {
                                        Id = gdc.Idhelpdesk,
                                        Tipo = md.Tipo.GetDescription(),
                                        NomeSolicitante = usslc.Nome,
                                        InicioResposta = gdc.InicioResposta,
                                        DataAbertura = gdc.DataAbertura,
                                        DataVencimento = gdc.DataVencimento,
                                        NomeResponsavel = usrsp.Nome,
                                        Clausula = cl.Nome,
                                        Status = gdc.StatusChamado
                                    })
                                    .ToDataTableResponseAsync(request);
#pragma warning restore CA1305 // Specify IFormatProvider

                return Ok(result);
            }

            return NoContent();
        }
    }
}
