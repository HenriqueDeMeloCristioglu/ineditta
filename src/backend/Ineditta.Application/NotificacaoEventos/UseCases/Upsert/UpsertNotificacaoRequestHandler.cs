using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.NotificacaoEventos.Entities;
using Ineditta.Application.NotificacaoEventos.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.NotificacaoEventos.UseCases.Upsert
{
    public class UpsertNotificacaoRequestHandler : BaseCommandHandler, IRequestHandler<UpsertNotificacaoRequest, Result>
    {
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly IEventoRepository _eventosRepository;

        public UpsertNotificacaoRequestHandler(IUnitOfWork unitOfWork, INotificacaoRepository notificacaoRepository, IEventoRepository eventosRepository) : base(unitOfWork)
        {
            _notificacaoRepository = notificacaoRepository;
            _eventosRepository = eventosRepository;
        }

        public async Task<Result> Handle(UpsertNotificacaoRequest request, CancellationToken cancellationToken)
        {
            return await IncluirAsync(request, cancellationToken);
        }

        private async Task<Result> IncluirAsync(UpsertNotificacaoRequest request, CancellationToken cancellationToken)
        {
            var eventoAssociado = await _eventosRepository.ObterPorIdAsync(request.EventoId);

            if (eventoAssociado is null)
            {
                return Result.Failure("O evento associado enviado não foi encontrado.");
            }

            if (request.Notificados is null)
            {
                return Result.Failure("Você deve forncecer uma lista de emails a serem notificados.");
            }

            List<Email> notificados = new();

            if (request.Notificados.Any())
            {
                foreach (var emailString in request.Notificados)
                {
                    var email = Email.Criar(emailString);

                    if (email.IsFailure)
                    {
                        return email;
                    }

                    notificados.Add(email.Value);
                }
            }

            var notificacao = Notificacao.Criar(request.EventoId, notificados);

            if (notificacao.IsFailure)
            {
                return notificacao;
            }

            await _notificacaoRepository.IncluirAsync(notificacao.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success(notificacao);
        }

    }
}
