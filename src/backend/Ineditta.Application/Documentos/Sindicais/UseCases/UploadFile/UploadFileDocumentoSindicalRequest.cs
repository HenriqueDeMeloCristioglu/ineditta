using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.FileStorage;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UploadFile
{
    public class UploadFileDocumentoSindicalRequest : IRequest<Result<FileInfoDto>>
    {
        public IEnumerable<FileDto>? Arquivo { get; set; } = null!;
    }
}
