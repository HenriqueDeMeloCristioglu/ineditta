using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Events.DocumentoCriado;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Extensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.NotificarCriacao
{
    public class NotificarCriacaoRequestHandler : BaseCommandHandler, IRequestHandler<NotificarCriacaoRequest, Result>
    {
        private readonly IMessagePublisher _messagePublisher;
        public NotificarCriacaoRequestHandler(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher) : base(unitOfWork)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task<Result> Handle(NotificarCriacaoRequest request, CancellationToken cancellationToken)
        {
            foreach (var usuariosIds in request.UsuariosParaNotificarIds)
            {
                DocumentoCriadoEvent message = new(request.DocumentoId, usuariosIds);

                await _messagePublisher.SendAsync(message, cancellationToken);
            }

            return Result.Success();
        }
    }
}
