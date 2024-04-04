using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Aprovar
{
    public class AprovarDocumentoLocalizadoRequestHandler : BaseCommandHandler, IRequestHandler<AprovarDocumentoLocalizadoRequest, Result>
    {
        private readonly IDocumentoLocalizadoRepository _documentoLocalizadoRepository;
        public AprovarDocumentoLocalizadoRequestHandler(IUnitOfWork unitOfWork, IDocumentoLocalizadoRepository documentoLocalizadoRepository) : base(unitOfWork)
        {
            _documentoLocalizadoRepository = documentoLocalizadoRepository;
        }
        public async Task<Result> Handle(AprovarDocumentoLocalizadoRequest request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return Result.Failure("O id deve ser maior ou igual a 0");

            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(request.Id);

            if (documentoLocalizado is null) return Result.Failure("Documento a ser aprovado não encontrado.");

            documentoLocalizado.Aprovar();

            await _documentoLocalizadoRepository.AtualizarAsync(documentoLocalizado);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
