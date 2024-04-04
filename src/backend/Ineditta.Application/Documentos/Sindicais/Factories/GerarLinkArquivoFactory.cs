using Ineditta.Application.SharedKernel.Frontend;
using Ineditta.BuildingBlocks.Core.Tokens;

using Microsoft.Extensions.Options;

namespace Ineditta.Application.Documentos.Sindicais.Factories
{
    public class GerarLinkArquivoFactory
    {
        private readonly FrontendConfiguration _frontendConfiguration;
        private readonly ITokenService _tokenService;

        public GerarLinkArquivoFactory(IOptions<FrontendConfiguration> frontendConfiguration, ITokenService tokenService)
        {
            _frontendConfiguration = frontendConfiguration?.Value ?? throw new ArgumentNullException(nameof(frontendConfiguration));
            _tokenService = tokenService;
        }

        public string Criar(long documentoId)
        {
            var dateExpiracao = DateTimeOffset.Now.AddYears(1) - DateTimeOffset.Now;

            var token = _tokenService.Create(dateExpiracao);

            return $"{_frontendConfiguration.Url}/api/v1/documentos-sindicais/{documentoId}/arquivos?code={token}";
        }
    }
}
