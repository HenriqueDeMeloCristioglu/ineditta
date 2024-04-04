using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalCriado
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class DocumentoSindicalCriadoEventHandler : BaseCommandHandler, IRequestHandler<DocumentoSindicalCriadoEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<DocumentoSindicalCriadoEventHandler> _logger;

        public DocumentoSindicalCriadoEventHandler(IUnitOfWork unitOfWork,
                                                   IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                   IMessagePublisher messagePublisher,
                                                   ILogger<DocumentoSindicalCriadoEventHandler> logger) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(DocumentoSindicalCriadoEvent message, CancellationToken cancellationToken = default)
        {
            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(message.IADocumentoSindicalId);

            if (iaDocumentoSindical is null)
            {
                _logger.LogError("Documento sindical IA não foi encontrado");
                return Result.Failure("Documento sindical IA não foi encontrado");
            }

            iaDocumentoSindical.QuebrarClausulas();

            await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);

            await CommitAsync(cancellationToken);

            DocumentoSindicalQuebraClausulaIniciadaEvent evento = new(iaDocumentoSindical.Id);

            await _messagePublisher.SendAsync(evento, cancellationToken);

            return Result.Success();
        }
    }
}
