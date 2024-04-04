using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.QuebraClausulaReprocessada
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class QuebraClausulaReprocessadaEventHandler : BaseCommandHandler, IRequestHandler<QuebraClausulaReprocessadaEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<QuebraClausulaReprocessadaEventHandler> _logger;

        public QuebraClausulaReprocessadaEventHandler(IUnitOfWork unitOfWork,
                                                      IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                      IIAClausulaRepository iAClausulaRepository,
                                                      IMessagePublisher messagePublisher,
                                                      ILogger<QuebraClausulaReprocessadaEventHandler> logger) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(QuebraClausulaReprocessadaEvent message, CancellationToken cancellationToken = default)
        {
            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(message.IADocumentoSindicalId);

            if (iaDocumentoSindical is null)
            {
                _logger.LogError("Documento sindical IA não foi encontrado");
                return Result.Failure("Documento sindical IA não foi encontrado");
            }

            var iAClausulas = await _iAClausulaRepository.ObterTodosPorIADocumentoIdAsync(iaDocumentoSindical.Id);

            if (iAClausulas is not null)
            {
                await _iAClausulaRepository.DeleteAsync(iAClausulas);
            }

            await CommitAsync(cancellationToken);

            DocumentoSindicalQuebraClausulaIniciadaEvent evento = new(iaDocumentoSindical.Id);

            await _messagePublisher.SendAsync(evento, cancellationToken);

            return Result.Success();
        }
    }
}
