using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.DocumentosLocalizados.Entities;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.FileStorage;

using MediatR;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Upsert
{
    public class UpsertDocumentoLocalizadoRequestHandler : BaseCommandHandler, IRequestHandler<UpsertDocumentoLocalizadoRequest, Result>
    {
        private readonly IDocumentoLocalizadoRepository _documentoLocalizadoRepository;
        private readonly IFileStorage _fileStorage;
        public UpsertDocumentoLocalizadoRequestHandler(IUnitOfWork unitOfWork, IDocumentoLocalizadoRepository documentoLocalizadoRepository, IFileStorage fileStorage) : base(unitOfWork)
        {
            _documentoLocalizadoRepository = documentoLocalizadoRepository;
            _fileStorage = fileStorage;
        }

        public async Task<Result> Handle(UpsertDocumentoLocalizadoRequest request, CancellationToken cancellationToken)
        {
            if (request.Id is not null)
            {
                return await AtualizarAsync(request, cancellationToken);
            }
            else
            {
                return await IncluirAsync(request, cancellationToken);
            }
        }

        public async Task<Result> IncluirAsync(UpsertDocumentoLocalizadoRequest request, CancellationToken cancellationToken)
        {
            Guid guid = Guid.NewGuid();

            if (request.Arquivo is null) return Result.Failure("Nenhum arquivo fornecido");

            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                await request.Arquivo.CopyToAsync(ms, cancellationToken);
                bytes = ms.ToArray();
            }

            var resultUpload = await _fileStorage.AddAsync(new FileDto(guid.ToString() + Path.GetExtension(request.Arquivo.FileName), bytes, DocumentoSindical.PastaDocumento), cancellationToken);

            if (resultUpload.IsFailure)
            {
                return Result.Failure("Falha ao tentar realizar o upload do arquivo");
            }

            var documentoLocalizado = DocumentoLocalizado.Criar(
                request.Arquivo.FileName,
                request.Origem,
                resultUpload.Value.FileName,
                request.Situacao ?? Situacao.NaoAprovado,
                request.DataAprovacao,
                request.IdLegado,
                request.Uf
            );

            if (documentoLocalizado.IsFailure)
            {
                return documentoLocalizado;
            }

            await _documentoLocalizadoRepository.IncluirAsync(documentoLocalizado.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> AtualizarAsync(UpsertDocumentoLocalizadoRequest request, CancellationToken cancellationToken)
        {
            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(request.Id ?? 0);

            if (documentoLocalizado is null) return Result.Failure("Documento Localizado não encontrado");

            var atualizarResultado = documentoLocalizado.Atualizar(
                request.NomeDocumento,
                request.Origem,
                request.CaminhoArquivo ?? documentoLocalizado.CaminhoArquivo,
                request.Situacao ?? documentoLocalizado.Situacao,
                request.DataAprovacao,
                request.Referenciado,
                request.IdLegado,
                request.Uf
            );

            if (atualizarResultado.IsFailure)
            {
                return atualizarResultado;
            }

            await _documentoLocalizadoRepository.AtualizarAsync(documentoLocalizado);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
