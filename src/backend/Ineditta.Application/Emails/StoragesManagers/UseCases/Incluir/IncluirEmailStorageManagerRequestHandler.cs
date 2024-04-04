
using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.StoragesManagers.Entities;
using Ineditta.Application.Emails.StoragesManagers.Repositories;
using Ineditta.Application.Emails.StoragesManagers.Services.LimparCaixasDeSaida;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.Emails.StoragesManagers.UseCases.Incluir
{
    public class IncluirEmailStorageManagerRequestHandler : BaseCommandHandler, IRequestHandler<IncluirEmailStorageManagerRequest, Result>
    {
        private readonly IEmailStorageManagerRepository _emailStorageManagerRepository;
        private readonly IRemoverDaCaixaDeSaidaService _removerDaCaixaDeSaidaService;
        public IncluirEmailStorageManagerRequestHandler(IUnitOfWork unitOfWork, IEmailStorageManagerRepository emailStorageManagerRepository, IRemoverDaCaixaDeSaidaService removerDaCaixaDeSaidaService) : base(unitOfWork)
        {
            _emailStorageManagerRepository = emailStorageManagerRepository;
            _removerDaCaixaDeSaidaService = removerDaCaixaDeSaidaService;
        }

        public async Task<Result> Handle(IncluirEmailStorageManagerRequest request, CancellationToken cancellationToken)
        {
            var from = Email.Criar(request.From);
            if (from.IsFailure)
            {
                return from;
            }

            var to = Email.Criar(request.To);
            if (to.IsFailure)
            {
                return to;
            }

            var emailStorageManager = EmailStorageManager.Criar(from.Value, to.Value, request.MessageId, request.Assunto, request.Enviado ?? false, request.RequestData);
            if (emailStorageManager.IsFailure)
            {
                return emailStorageManager;
            }

            await _emailStorageManagerRepository.IncluirAsync(emailStorageManager.Value);

            await CommitAsync(cancellationToken);

            if (request.Enviado is not null && request.Enviado == true)
            {
                await _removerDaCaixaDeSaidaService.RemoverAsync(request.MessageId, cancellationToken);
            }

            return Result.Success();
        }
    }
}
