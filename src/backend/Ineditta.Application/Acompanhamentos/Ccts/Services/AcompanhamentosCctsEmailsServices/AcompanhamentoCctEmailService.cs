using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Renderers.Html;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEmailsServices
{
    public class AcompanhamentoCctEmailService : IAcompanhamentoCctEmailService
    {
        private readonly IEmailSender _emailSender;

        public AcompanhamentoCctEmailService(IHtmlRenderer _, IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async ValueTask<Result<bool, Error>> EnviarEmailContatoAsync(IEnumerable<string> emails, string template, string assunto, CancellationToken cancellationToken = default)
        {
            var result = await _emailSender.SendAsync(new SendEmailRequestDto(emails, assunto, template), cancellationToken);

            return result.IsFailure ? Result.Failure<bool, Error>(result.Error) : Result.Success<bool, Error>(true);
        }
    }
}
