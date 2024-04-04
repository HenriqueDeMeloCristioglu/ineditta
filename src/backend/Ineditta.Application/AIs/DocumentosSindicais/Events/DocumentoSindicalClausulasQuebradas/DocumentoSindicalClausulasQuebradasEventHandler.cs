using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClassificacaoIniciada;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class DocumentoSindicalClausulasQuebradasEventHandler : BaseCommandHandler, IRequestHandler<DocumentoSindicalClausulasQuebradasEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<DocumentoSindicalClausulasQuebradasEventHandler> _logger;

        public DocumentoSindicalClausulasQuebradasEventHandler(IUnitOfWork unitOfWork,
                                                               IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                               IIAClausulaRepository iAClausulaRepository,
                                                               IMessagePublisher messagePublisher,
                                                               ILogger<DocumentoSindicalClausulasQuebradasEventHandler> logger) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(DocumentoSindicalClausulasQuebradasEvent message, CancellationToken cancellationToken = default)
        {
            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(message.IADocumentoSindicalId);

            if (iaDocumentoSindical is null)
            {
                _logger.LogError("Documento sindical IA não foi encontrado");
                return Result.Failure("Documento sindical IA não foi encontrado");
            }

            var iAClausulas = await _iAClausulaRepository.ObterTodosPorIADocumentoIdAsync(iaDocumentoSindical.Id, true);

            if (iAClausulas is null || !iAClausulas.Any())
            {
                _logger.LogError("Nenhuma cláusula vinculada ao documento sindical foi encontrada");
                iaDocumentoSindical.RegistrarErro("Nenhuma cláusula vinculada ao documento sindical foi encontrada");
                await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);
            } else
            {
                foreach (var iAClausula in iAClausulas)
                {
                    DocumentoSindicalClassificacaoIniciadaEvent evento = new(iAClausula.Id);

                    await _messagePublisher.SendAsync(evento, cancellationToken);
                }
            }

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
