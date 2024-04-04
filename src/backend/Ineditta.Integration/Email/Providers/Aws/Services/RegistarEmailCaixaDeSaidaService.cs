using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.UseCases.Incluir;
using Ineditta.Integration.Email.Dtos;

using MediatR;

namespace Ineditta.Integration.Email.Providers.Aws.Services
{
    public class RegistarEmailCaixaDeSaidaService
    {
        private readonly IMediator _mediator;

        public RegistarEmailCaixaDeSaidaService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<Result> Handle(SendEmailRequestDto request, string messageId, CancellationToken cancellationToken)
        {

            if (request.To is not null && request.To.Any())
            {
                foreach (var email in request.To)
                {
                    var incluirEmailCaixaDeSaidaRequest = new IncluirEmailCaixaDeSaidaRequest
                    {
                        Email = email,
                        Assunto = request.Subject,
                        Template = request.Body,
                        MessageId = messageId
                    };

                    await _mediator.Send(incluirEmailCaixaDeSaidaRequest, cancellationToken);
                }
            }

            if (request.Cc is not null && request.Cc.Any())
            {
                foreach (var email in request.Cc)
                {
                    var incluirEmailCaixaDeSaidaRequest = new IncluirEmailCaixaDeSaidaRequest
                    {
                        Email = email,
                        Assunto = request.Subject,
                        Template = request.Body,
                        MessageId = messageId
                    };

                    await _mediator.Send(incluirEmailCaixaDeSaidaRequest, cancellationToken);
                }
            }

            return Result.Success();
        }
    }
}
