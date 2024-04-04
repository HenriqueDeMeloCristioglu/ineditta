using System.Net;

using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Services.Mte;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.Mte.Configurations;
using Ineditta.Integration.Mte.Factories;
using Ineditta.Integration.Mte.Services.Http;

using Microsoft.Extensions.Options;

namespace Ineditta.Integration.Mte.Services
{
    public class MteService : IMteService
    {
        private readonly MteConfiguration _configuration;
        private readonly MteHttpClient _mteHttpClient;
        private const int RetryCount = 3;

        public MteService(IOptions<MteConfiguration> configuration,
                          MteHttpClient mteHttpClient)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _mteHttpClient = mteHttpClient;
        }

        public async ValueTask<Result<string, Error>> ObterHtmlContrato(string numeroSolicitacaoMte, CancellationToken cancellationToken = default)
        {
            return await RetryPolicyFactory.ExecuteWithRetryFailureAsync(async () =>
            {
                if (string.IsNullOrEmpty(numeroSolicitacaoMte))
                {
                    return Result.Failure<string, Error>(Errors.General.Business("Número de solicitação do MTE deve ser informado."));
                }

                var solicitacaoMte = numeroSolicitacaoMte.Trim();

                try
                {
                    var textoContrato = await _mteHttpClient.GetStringAsync($"{_configuration.Url}{solicitacaoMte}", cancellationToken);

                    if (textoContrato.IsFailure)
                    {
                        return Result.Failure<string, Error>(textoContrato.Error);
                    }

                    if (string.IsNullOrEmpty(textoContrato.Value))
                    {
                        return Result.Failure<string, Error>(Errors.General.Business($"Não foi possível obter o html da solicitação {numeroSolicitacaoMte} MTE."));
                    }

                    return Result.Success<string, Error>(textoContrato.Value);

                }
                catch (HttpRequestException e) when (e.StatusCode.HasValue && e.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return Result.Failure<string, Error>(Errors.Http.TooManyRequests());
                }
                catch (HttpRequestException e) when (e.StatusCode.HasValue && (int)e.StatusCode >= 500 && (int)e.StatusCode <= 599)
                {
                    return Result.Failure<string, Error>(Errors.Http.ErrorCode());
                }
                catch (TaskCanceledException e)
                {
                    return Result.Failure<string, Error>(Errors.Http.Timeout(e.Message));
                }
                catch (HttpRequestException e)
                {
                    return Result.Failure<string, Error>(Errors.General.InternalServerError($"Não foi possível processar a solicitação: {e.StatusCode} - {e.Message}"));
                }
                catch (Exception ex)
                {
                    return Result.Failure<string, Error>(Errors.General.InternalServerError($"MTE indisponível no momento: {ex.Message}"));
                }
            }, RetryCount);
        }
    }
}
