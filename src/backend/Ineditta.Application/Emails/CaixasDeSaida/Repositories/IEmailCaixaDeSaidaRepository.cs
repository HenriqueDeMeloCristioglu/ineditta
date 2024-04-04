using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Entities;

namespace Ineditta.Application.Emails.CaixasDeSaida.Repositories
{
    public interface IEmailCaixaDeSaidaRepository
    {
        ValueTask<Result> IncluirAsync(EmailCaixaDeSaida emailCaixaDeSaida);
        ValueTask<Result<IEnumerable<EmailCaixaDeSaida>>> ObterPorMessageId(string messageId);
        ValueTask<Result<IEnumerable<EmailCaixaDeSaida>>> ObterTodosAsync();
        ValueTask<Result> DeletarAsync(EmailCaixaDeSaida emailCaixaDeSaida);
    }
}
