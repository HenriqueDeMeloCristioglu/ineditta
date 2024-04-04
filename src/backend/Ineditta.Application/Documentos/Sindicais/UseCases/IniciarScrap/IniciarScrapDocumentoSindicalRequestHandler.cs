using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Events.DocumentoSisapCriadoJobIa;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.IniciarScrap
{
    public class IniciarScrapDocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<IniciarScrapDocumentoSindicalRequest, Result>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;

        public IniciarScrapDocumentoSindicalRequestHandler(IUnitOfWork unitOfWork, IDocumentoSindicalRepository documentoSindicalRepository, IMessagePublisher messagePublisher, IIADocumentoSindicalRepository iADocumentoSindicalRepository) : base(unitOfWork)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _messagePublisher = messagePublisher;
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
        }

        public async Task<Result> Handle(IniciarScrapDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            DocumentoSindical? documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(request.DocumentoId);

            if (documentoSindical == null) return Result.Failure("Documento fornecido para o scrap não encontrado");
            if (documentoSindical.DataAprovacao == null) return Result.Failure("Documento fornecido para o scrap não está aprovado");

            bool scrapJaIniciadoNesseDocumento = await _iADocumentoSindicalRepository.ExistePorDocumentoReferenciaIdAsync(documentoSindical.Id);
            if (scrapJaIniciadoNesseDocumento) return Result.Failure("Já existe scrap iniciado para este documento.");

            DocumentoSisapCriadoJobIaEvent message = new(documentoSindical.Id);
            await _messagePublisher.SendAsync(message, cancellationToken);

            return Result.Success();
        }
    }
}
