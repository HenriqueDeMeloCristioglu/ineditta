using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Microsoft.AspNetCore.Authorization;
using Ineditta.API.ViewModels.EmailsStoragesManagers.ViewModels;
using Ineditta.Integration.Email.Providers.Aws.Filters;
using Ineditta.BuildingBlocks.Core.Web.API.Attributes;
using Ineditta.Integration.Email.Providers.Aws.Models;
using Newtonsoft.Json;
using Amazon.SimpleEmail;
using Ineditta.Integration.Email.Configurations;
using Microsoft.Extensions.Options;
using Ineditta.Application.Emails.StoragesManagers.Entities;
using Ineditta.Application.Emails.StoragesManagers.UseCases.Incluir;
using Ineditta.API.Builders.AcompanhamentosCcts;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.API.ViewModels.AcompanhamentosCct.Requests;
using System.Net.Mime;
using Ineditta.API.Builders.Emails.StorageManagers;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1.EmailsControllers
{
    [Route("v{version:apiVersion}/emails-storages-managers")]
    [ApiController]
    public class EmailStorageManagerController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly EmailConfiguration _emailConfiguration;
        public EmailStorageManagerController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, HttpClient httpClient, IOptions<EmailConfiguration> emailConfiguration) : base(mediator, requestStateValidator)
        {
            _context = context;
            _httpClient = httpClient;
            _emailConfiguration = emailConfiguration?.Value ?? throw new ArgumentNullException(nameof(emailConfiguration));
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

            var result = await _context.EmailStorageManager
                .Select(em => new EmailStorageManagerDataTableViewModel
                {
                    To = em.To.Valor,
                    From = em.From.Valor,
                    Assunto = em.Assunto,
                    Enviado = em.Enviado ? "Sim" : "Não",
                    DataInclusao = em.DataInclusao,
                })
                .OrderByDescending(x => x.DataInclusao)
                .ToDataTableResponseAsync(request);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }


        [HttpPost]
        [ReadRequestBody]
        [ServiceFilter(typeof(AwsValidationFilter))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> IncluirAsync()
        {
            var requestBody = HttpContext.Items["RequestBody"]?.ToString() ?? string.Empty;

            var message = Amazon.SimpleNotificationService.Util.Message.ParseMessage(requestBody);

            if (message.IsSubscriptionType)
            {
                await _httpClient.GetAsync(message.SubscribeURL);
                return NoContent();
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var request = JsonConvert.DeserializeObject<AmazonSesNotificationRequest>(requestBody, settings);

            if (request is null)
            {
                return BadRequestFromErrorMessage("Invalid payload");
            }

            if (request.Message is null || request.Message.Mail is null || request.Message.Mail.Source is null || request.Message.Mail.Destination is null || !request.Message.Mail.Destination.Any() || request.Message.Mail.CommonHeaders is null || request.Message.Mail.CommonHeaders.Subject is null)
            {
                return BadRequestFromErrorMessage("Invalid payload");
            }

            bool enviado = request.Message.NotificationType == NotificationType.Delivery;

#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
            var sendRequest = new IncluirEmailStorageManagerRequest
            {
                Assunto = request.Message.Mail.CommonHeaders.Subject,
                To = request.Message.Mail.Destination.First(),
                From = _emailConfiguration.Aws.Source,
                MessageId = request.Message.Mail.MessageId,
                Enviado = enviado,
                RequestData = JsonConvert.SerializeObject(request)
            };
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"

            return await Dispatch(sendRequest);
        }

        [HttpPost("relatorios")]
        [Produces(MediaTypeNames.Application.Octet)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterRelatorio()
        {
            if (Request.Headers.Accept != MediaTypeNames.Application.Octet)
            {
                return NotAcceptable();
            }

            var emails = await (from em in _context.EmailStorageManager
                                select em)
                                .ToListAsync();

            if (emails is null)
            {
                return NoContent();
            }

            var relatorio = RelatorioExcelEmailStorageManageBuilder.Criar(emails);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var fileName = $"relatório_emails_enviados_{date.Day}_{date.Month}_{date.Year}";

            return DownloadExcel(fileName, relatorio);
        }
    }
}
