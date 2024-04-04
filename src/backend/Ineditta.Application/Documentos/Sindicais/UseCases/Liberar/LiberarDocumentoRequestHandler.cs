using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;


namespace Ineditta.Application.Documentos.Sindicais.UseCases.Liberar
{
    public class LiberarDocumentoRequestHandler : BaseCommandHandler, IRequestHandler<LiberarDocumentoSindicalRequest, Result>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;

        public LiberarDocumentoRequestHandler(IUnitOfWork unitOfWork, IDocumentoSindicalRepository documentoSindicalRepository) : base(unitOfWork)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
        }

        public async Task<Result> Handle(LiberarDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var documento = await _documentoSindicalRepository.ObterPorIdAsync(request.Id);

            if (documento == null)
            {
                return Result.Failure("Documento não encontrado");
            }

            var result = documento.Liberar();

            if (result.IsFailure)
            {
                return result;
            }

            await _documentoSindicalRepository.AtualizarAsync(documento, cancellationToken);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
