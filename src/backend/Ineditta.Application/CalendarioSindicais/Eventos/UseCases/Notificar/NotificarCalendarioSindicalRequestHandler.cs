using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Services;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Notificar
{
    public sealed class NotificarCalendarioSindicalRequestHandler : BaseCommandHandler, IRequestHandler<NotificarCalendarioSindicalRequest, Result<Unit, Error>>
    {
        private readonly INotificacaoCalendarioSindicalService _notificacaoCalendarioSindical;
        public NotificarCalendarioSindicalRequestHandler(IUnitOfWork unitOfWork, INotificacaoCalendarioSindicalService notificacaoCalendarioSindical) : base(unitOfWork)
        {
            _notificacaoCalendarioSindical = notificacaoCalendarioSindical;
        }
        public async Task<Result<Unit, Error>> Handle(NotificarCalendarioSindicalRequest request, CancellationToken cancellationToken)
        {
            var result = await _notificacaoCalendarioSindical.EnviarNotificacoesAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<Unit, Error>(result.Error);
            }

            await CommitAsync(cancellationToken);

            return Result.Success<Unit, Error>(Unit.Value);
        }
    }
}
