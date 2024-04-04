using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Services
{
    public interface INotificacaoCalendarioSindicalService
    {
        ValueTask<Result<bool, Error>> EnviarNotificacoesAsync(CancellationToken cancellationToken);
    }
}
