using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.FileStorage;

using MediatR;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Deletar
{
    public class DeletarDocumentoLocalizadoRequestHandler : BaseCommandHandler, IRequestHandler<DeletarDocumentoLocalizadoRequest, Result>
    {
        private readonly IDocumentoLocalizadoRepository _documentoLocalizadoRepository;
        private readonly IFileStorage _fileStorage;
        public DeletarDocumentoLocalizadoRequestHandler(IUnitOfWork unitOfWork, IDocumentoLocalizadoRepository documentoLocalizadoRepository, IFileStorage fileStorage) : base(unitOfWork)
        {
            _documentoLocalizadoRepository = documentoLocalizadoRepository;
            _fileStorage = fileStorage;
        }
        public async Task<Result> Handle(DeletarDocumentoLocalizadoRequest request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return Result.Failure("O id deve ser maior ou igual a 0");

            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(request.Id);

            if (documentoLocalizado is null) return Result.Failure("Documento a ser DELETADO não encontrado.");

            var deletarResultado = await _fileStorage.DeleteAsync(documentoLocalizado.CaminhoArquivo, cancellationToken);

            if (deletarResultado.IsFailure)
            {
                return deletarResultado;
            }

            await _documentoLocalizadoRepository.DeletarAsync(documentoLocalizado);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}