using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Services
{
    public interface ICalendarioSindicalGeradorService
    {
        ValueTask<Result<bool, Error>> GerarAsync(CancellationToken cancellationToken);
        ValueTask<Result<bool, Error>> GerarAgendaEventosAsync(CancellationToken cancellationToken);
    }
}
