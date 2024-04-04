using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.Documentos.Localizados.Services
{
    public interface IDocumentoLocalizadoService
    {
        ValueTask<Result<byte[], Error>> ObterBytesPorDocumentoId(long documentoLocalizadoId, CancellationToken cancellationToken = default);
    }
}
