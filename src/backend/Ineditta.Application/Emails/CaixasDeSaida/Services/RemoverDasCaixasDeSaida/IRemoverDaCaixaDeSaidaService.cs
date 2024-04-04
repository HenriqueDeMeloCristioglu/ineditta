using CSharpFunctionalExtensions;

namespace Ineditta.Application.Emails.StoragesManagers.Services.LimparCaixasDeSaida
{
    public interface IRemoverDaCaixaDeSaidaService
    {
        ValueTask<Result> RemoverAsync(string messageId, CancellationToken cancellationToken);
    }
}
