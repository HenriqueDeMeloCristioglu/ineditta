using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.UseCases.Incluir;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.Email.Configurations;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;
using Ineditta.Integration.Email.Providers.Mailhog.Models.NotificationEmailStorage;

using MailKit.Net.Smtp;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

namespace Ineditta.Integration.Email.Providers.Mailhog.Services
{
    public class MailhogEmailSender : IEmailSender
    {
        private readonly EmailConfiguration _configuration;
        private readonly ILogger<MailhogEmailSender> _logger;
        private readonly MailhogNotificationEmailService _mailhogNotificationEmailService;
        private readonly IMediator _mediator;
        public MailhogEmailSender(IOptions<EmailConfiguration> configuration, ILogger<MailhogEmailSender> logger, MailhogNotificationEmailService mailhogNotificationEmailService, IMediator mediator)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _mailhogNotificationEmailService = mailhogNotificationEmailService;
            _mediator = mediator;
        }

        public async ValueTask<Result<SendEmailResponseDto, Error>> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            foreach (var email in request.To)
            {
                var incluirEmailCaixaDeSaidaRequest = new IncluirEmailCaixaDeSaidaRequest
                {
                    Email = email,
                    Assunto = request.Subject,
                    Template = request.Body
                };

                await _mediator.Send(incluirEmailCaixaDeSaidaRequest, cancellationToken);
            }

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Não responda", _configuration.Smtp.FromEmail));
            mailMessage.Subject = request.Subject;
            mailMessage.Body = request.IsHtml ? new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = request.Body
            } :
            new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = request.Body
            };


            foreach (var to in request.To)
            {
                mailMessage.To.Add(new MailboxAddress(to, to));
            }

            if (request.Cc != null && request.Cc.Any())
            {
                foreach (var cc in request.Cc)
                {
                    mailMessage.Cc.Add(new MailboxAddress(cc, cc));
                }
            }

            using var smtpClient = new SmtpClient();

            try
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
#pragma warning disable CA5359 // Do Not Disable Certificate Validation
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
#pragma warning restore CA5359 // Do Not Disable Certificate Validation
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections

                await smtpClient!.ConnectAsync(_configuration.Smtp.Host, _configuration.Smtp.Port, false, cancellationToken);
                await smtpClient.SendAsync(mailMessage, cancellationToken);
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Não foi possível enviar o email");

                return Result.Failure<SendEmailResponseDto, Error>(Error.Create("smtp.exception.generic", "Não foi possível enviar o email"));
            }

            foreach (var to in request.To)
            {
                var mailhogNotificationEmailServiceRequest = new MailhogNotificationEmailServiceRequest
                {
                    Assunto = request.Subject,
                    Enviado = true,
                    From = _configuration.Smtp.FromEmail,
                    MessageId = "Fake Id",
                    To = to
                };
                var notificationResult = await _mailhogNotificationEmailService.Handle(mailhogNotificationEmailServiceRequest);

                if (notificationResult.IsFailure)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogInformation($"Erro ao notificar email: {notificationResult.Error}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }

            return Result.Success<SendEmailResponseDto, Error>(new SendEmailResponseDto() { MessageId = Guid.NewGuid().ToString() });
        }
    }
}