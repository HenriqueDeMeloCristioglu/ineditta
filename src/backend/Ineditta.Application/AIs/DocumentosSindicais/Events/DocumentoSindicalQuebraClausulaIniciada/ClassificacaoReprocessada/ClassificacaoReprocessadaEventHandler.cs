using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.ClassificacaoReprocessada
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class ClassificacaoReprocessadaEventHandler : BaseCommandHandler, IRequestHandler<ClassificacaoReprocessadaEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ClassificacaoReprocessadaEventHandler> _logger;

        public ClassificacaoReprocessadaEventHandler(IUnitOfWork unitOfWork,
                                                     IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                      IIAClausulaRepository iAClausulaRepository,
                                                     IMessagePublisher messagePublisher,
                                                     ILogger<ClassificacaoReprocessadaEventHandler> logger) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(ClassificacaoReprocessadaEvent message, CancellationToken cancellationToken = default)
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
                foreach (var iAClausula in iAClausulas)
                {
                    iAClausula.LimparClassificacao();

                    await _iAClausulaRepository.AtualizarAsync(iAClausula);
                }
            }

            await CommitAsync(cancellationToken);

            DocumentoSindicalClausulasQuebradasEvent evento = new(iaDocumentoSindical.Id);

            await _messagePublisher.SendAsync(evento, cancellationToken);

            return Result.Success();
        }
    }
}
