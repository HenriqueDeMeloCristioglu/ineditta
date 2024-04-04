using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalCriado;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Upsert
{
    public class UpsertDocumentoSindicalIARequestHandler : BaseCommandHandler, IRequestHandler<UpsertDocumentoSindicalIARequest, Result>
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IMessagePublisher _messagePublisher;

        public UpsertDocumentoSindicalIARequestHandler(IUnitOfWork unitOfWork, IIADocumentoSindicalRepository iADocumentoSindicalRepository, IMessagePublisher messagePublisher) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result> Handle(UpsertDocumentoSindicalIARequest request, CancellationToken cancellationToken)
        {
            return request.Id <= 0 ? await Incluir(request, cancellationToken) : await Atualizar(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertDocumentoSindicalIARequest request, CancellationToken cancellationToken)
        {
            var iaDocumentoSindical = IADocumentoSindical.Criar(request.DocumentoReferenciaId, request.Status);

            if (iaDocumentoSindical.IsFailure)
            {
                return iaDocumentoSindical;
            }

            await _iADocumentoSindicalRepository.IncluirAsync(iaDocumentoSindical.Value);

            await CommitAsync(cancellationToken);

            DocumentoSindicalCriadoEvent message = new(iaDocumentoSindical.Value.Id);
            await _messagePublisher.SendAsync(message, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertDocumentoSindicalIARequest request, CancellationToken cancellationToken)
        {
            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(request.Id);

            if (iaDocumentoSindical is null)
            {
                return Result.Failure("Documento Sindical IA não encontrado");
            }

            iaDocumentoSindical.Atualizar(request.DocumentoReferenciaId, request.Status, request.MotivoErro);

            await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
