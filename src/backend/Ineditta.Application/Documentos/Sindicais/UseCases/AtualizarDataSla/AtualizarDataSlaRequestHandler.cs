using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.AtualizarDataSla
{
    public class AtualizarDataSlaRequestHandler : BaseCommandHandler, IRequestHandler<AtualizarDataSlaRequest, Result>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        public AtualizarDataSlaRequestHandler(IUnitOfWork unitOfWork, IDocumentoSindicalRepository documentoSindicalRepository) : base(unitOfWork)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
        }
        public async Task<Result> Handle(AtualizarDataSlaRequest request, CancellationToken cancellationToken)
        {
            if (request.DocSindId is null)
            {
                return Result.Failure("Você deve fornecer o id do documento sindical");
            }

            var documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(request.DocSindId ?? 0);

            if (documentoSindical is null)
            {
                return Result.Failure("Documento sindical não encontrado");
            }

            var resultAtualizarDataSla = documentoSindical.AtualizarDataSla(request.NovaDataSla);

            if (resultAtualizarDataSla.IsFailure)
            {
                return resultAtualizarDataSla;
            }

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
