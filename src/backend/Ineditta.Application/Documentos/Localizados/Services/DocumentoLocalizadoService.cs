using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.FileStorage;

using Microsoft.Extensions.Options;

namespace Ineditta.Application.Documentos.Localizados.Services
{
    public class DocumentoLocalizadoService : IDocumentoLocalizadoService
    {

        private readonly IDocumentoLocalizadoRepository _documentoLocalizadoRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFileStorage _fileStorage;
        private readonly FileStorageConfiguration _fileStorageConfiguration;

        public DocumentoLocalizadoService(IDocumentoLocalizadoRepository documentoLocalizadoRepository,
                                     IHttpClientFactory httpClientFactory,
                                     IFileStorage fileStorage,
                                     IOptions<FileStorageConfiguration> fileStorageConfiguration)
        {
            _documentoLocalizadoRepository = documentoLocalizadoRepository;
            _httpClientFactory = httpClientFactory;
            _fileStorage = fileStorage;
            _fileStorageConfiguration = fileStorageConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileStorageConfiguration));
        }

        public async ValueTask<Result<byte[], Error>> ObterBytesPorDocumentoId(long documentoLocalizadoId, CancellationToken cancellationToken = default)
        {
            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(documentoLocalizadoId);
            if (documentoLocalizado is null)
            {
                return Result.Failure<byte[], Error>(Errors.General.NotFound(documentoLocalizadoId));
            }

            if (Uri.TryCreate(documentoLocalizado.CaminhoArquivo, UriKind.Absolute, out var uri))
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.GetAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return Result.Success<byte[], Error>(await response.Content.ReadAsByteArrayAsync(cancellationToken));
                }
            }

            var arquivo = await _fileStorage.GetAsync($"{_fileStorageConfiguration.Path}/{DocumentoSindical.PastaDocumento}/{documentoLocalizado.CaminhoArquivo}", cancellationToken);

            return arquivo.IsFailure ? Result.Failure<byte[], Error>(arquivo.Error) : Result.Success<byte[], Error>(arquivo.Value.Content);
        }
    }
}
