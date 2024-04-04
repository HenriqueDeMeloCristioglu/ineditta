using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEmailsServices
{
    public interface IAcompanhamentoCctEmailService
    {
        ValueTask<Result<bool, Error>> EnviarEmailContatoAsync(IEnumerable<string> emails, string template, string assunto, CancellationToken cancellationToken = default);
    }
}
