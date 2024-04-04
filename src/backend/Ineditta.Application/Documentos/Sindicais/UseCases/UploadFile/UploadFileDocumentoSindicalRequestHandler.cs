using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.FileStorage;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UploadFile
{
    public class UploadFileDocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<UploadFileDocumentoSindicalRequest, Result<FileInfoDto>>
    {
        private readonly IFileStorage _fileStorage;
        public UploadFileDocumentoSindicalRequestHandler(IUnitOfWork unitOfWork, IFileStorage fileStorage) : base(unitOfWork)
        {
            _fileStorage = fileStorage;

        }
        public async Task<Result<FileInfoDto>> Handle(UploadFileDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            if (request.Arquivo is null || !request.Arquivo.Any())
            {
                return Result.Failure<FileInfoDto>("Você precisa fornecer um arquivo");
            }

            Guid guid = Guid.NewGuid();

            var arquivo = request.Arquivo.ToList()[0];

            arquivo = new FileDto(guid.ToString() + arquivo.Name, arquivo.Content, DocumentoSindical.PastaDocumento);

            var extensao = Path.GetExtension(arquivo.Name);

            if (!ExtensoesAceitas.Extensoes.Contains(extensao))
            {
                return Result.Failure<FileInfoDto>("Tipo de arquivo não permitido");
            }

            var resultUpload = await _fileStorage.AddAsync(arquivo, cancellationToken);

            if (resultUpload.IsFailure)
            {
                return Result.Failure<FileInfoDto>("Falha ao tentar realizar o upload do arquivo");
            }

            return Result.Success(resultUpload.Value);
        }
    }
}
