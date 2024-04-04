using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Services.QuebraClausula;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada
{

#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class DocumentoSindicalQuebraClausulaIniciadaEventHandler : BaseCommandHandler, IRequestHandler<DocumentoSindicalQuebraClausulaIniciadaEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly IQuebraClausulaService _automacacaoQuebraClausulaService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<DocumentoSindicalQuebraClausulaIniciadaEventHandler> _logger;

        public DocumentoSindicalQuebraClausulaIniciadaEventHandler(IUnitOfWork unitOfWork,
                                                                   IDocumentoSindicalRepository documentoSindicalRepository,
                                                                   IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                                   IIAClausulaRepository iAClausulaRepository,
                                                                   IQuebraClausulaService automacacaoQuebraClausulaService,
                                                                   IMessagePublisher messagePublisher,
                                                                   ILogger<DocumentoSindicalQuebraClausulaIniciadaEventHandler> logger) : base(unitOfWork)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _automacacaoQuebraClausulaService = automacacaoQuebraClausulaService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(DocumentoSindicalQuebraClausulaIniciadaEvent message, CancellationToken cancellationToken = default)
        {
            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(message.IADocumentoSindicalId);

            if (iaDocumentoSindical is null)
            {
                _logger.LogError("Documento sindical IA não foi encontrado");
                return Result.Failure("Documento sindical IA não foi encontrado");
            }

            var documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(iaDocumentoSindical.DocumentoReferenciaId);

            if (documentoSindical is null)
            {
                _logger.LogError("Documento sindical não foi encontrado");
                return Result.Failure("Documento sindical não foi encontrado");
            }

            var result = await _automacacaoQuebraClausulaService.QuebrarContratoEmClausulas(documentoSindical.Id, cancellationToken);

            if (result.IsSuccess)
            {
                foreach (var clausula in result.Value.Clausulas)
                {
                    var iAClausula = IAClausula.Criar(clausula.Nome, clausula.Descricao, clausula.Grupo, clausula.SubGrupo, iaDocumentoSindical.Id, null, clausula.Numero, null, IAClausulaStatus.Consistente);

                    if (iAClausula.IsFailure)
                    {
                        _logger.LogError("Erro ao criar a cláusula: {Error}", iAClausula.Error);
                        Result.Failure(iAClausula.Error);
                    }

                    await _iAClausulaRepository.InserirAsync(iAClausula.Value);
                }

                iaDocumentoSindical.ConfirmarQuebraClausula(documentoSindical.Versao != "Registrado");
            } else
            {
                _logger.LogError("Erro ao quebrar o contrato em cláusulas: {Error}", result.Error.Message);
                iaDocumentoSindical.RegistrarErro(result.Error.Message);
            }

            await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);

            await CommitAsync(cancellationToken);

            if (documentoSindical.Versao == "Registrado" && result.IsSuccess)
            {
                DocumentoSindicalClausulasQuebradasEvent evento = new(iaDocumentoSindical.Id);

                await _messagePublisher.SendAsync(evento, cancellationToken);
            }

            return Result.Success();
        }
    }
}
