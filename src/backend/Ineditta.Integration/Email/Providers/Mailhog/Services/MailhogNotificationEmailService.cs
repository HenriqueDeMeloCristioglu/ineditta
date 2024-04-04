using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.StoragesManagers.UseCases.Incluir;
using Ineditta.Integration.Email.Providers.Mailhog.Models.NotificationEmailStorage;

using MediatR;

namespace Ineditta.Integration.Email.Providers.Mailhog.Services
{
    public class MailhogNotificationEmailService
    {
        private readonly IMediator _mediator;

        public MailhogNotificationEmailService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<Result> Handle(MailhogNotificationEmailServiceRequest request)
        {
            var sendRequest = new IncluirEmailStorageManagerRequest
            {
                Assunto = request.Assunto,
                To = request.To,
                From = request.From,
                MessageId = request.MessageId,
                Enviado = request.Enviado,
                RequestData = "MailHog"
            };

            var result = await _mediator.Send(sendRequest);

            if (result.IsFailure)
            {
                return result;
            }

            return Result.Success();
        }
    }
}
