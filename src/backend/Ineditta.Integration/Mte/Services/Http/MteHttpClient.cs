using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Ineditta.Integration.Mte.Services.Http
{
    public sealed class MteHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MteHttpClient> _logger;

        public MteHttpClient(HttpClient httpClient, ILogger<MteHttpClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async ValueTask<Result<string, Error>> GetStringAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            using (var content = response.Content)
            {
                try
                {
                    var textoContrato = await content.ReadAsStringAsync(cancellationToken);

                    if (string.IsNullOrEmpty(textoContrato))
                    {
                        _logger.LogError("Não foi possível obter HTML da página: {Url}", url);
                        return Result.Failure<string, Error>(Errors.General.Business($"Não foi possível obter HTML da página {url}."));
                    }

                    return Result.Success<string, Error>(textoContrato);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Erro na requisição HTTP: {Ex}", ex.Message);
                    return Result.Failure<string, Error>(Errors.General.Business($"Erro na requisição HTTP: {ex.Message}"));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ocorreu uma exception: {Ex}", ex.Message);
                    return Result.Failure<string, Error>(Errors.General.InternalServerError(ex.Message));
                }
            }            
        }
    }
}
