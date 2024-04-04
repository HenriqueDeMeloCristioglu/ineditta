using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.Application.Emails.StoragesManagers.Entities;
using Microsoft.EntityFrameworkCore;
using Ineditta.API.ViewModels.EmailsCaixasDeSaida.ViewModels;
using System.Net.Mime;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.Application.Emails.CaixasDeSaida.UseCases.ReenviarEmails;

namespace Ineditta.API.Controllers.V1.Emails
{
    [Route("v{version:apiVersion}/emails-caixas-de-saida")]
    [ApiController]
    public class EmailCaixaDeSaidaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        public EmailCaixaDeSaidaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<EmailStorageManager>), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept != DatatableMediaType.ContentType)
            {
                return NotAcceptable();
            }

            var result = await _context.EmailCaixaDeSaida
                .Select(e => new EmailCaixaDeSaidaDataTableViewModel
                {
                    Email = e.Email.Valor,
                    Assunto = e.Assunto,
                    DataInclusao = e.DataInclusao
                })
                .ToDataTableResponseAsync(request);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost("reenviar-emails")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ReenviarEmailsAsync(ReenviarEmailRequest request)
        {
            return await Dispatch(request);
        }
    }
}
