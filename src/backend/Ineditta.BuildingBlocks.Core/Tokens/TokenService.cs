using System.Globalization;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

using Polly;

namespace Ineditta.BuildingBlocks.Core.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly IDataProtectionProvider _provider;
        private readonly ILogger<ITokenService> _logger;
        public const string TokenName = "TokenService";

        public TokenService(IDataProtectionProvider provider, ILogger<ITokenService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public string Create(TimeSpan expireTimeFromNow)
        {
            var protector = _provider.CreateProtector(TokenName);

            var expiration = DateTime.UtcNow.Add(expireTimeFromNow);

            var data = $"{expiration:O}";

            return protector.Protect(data);
        }

        public bool IsExpired(string token)
        {
            var protector = _provider.CreateProtector(TokenName);

            try
            {
                var unprotectedToken = protector.Unprotect(token);

                if (string.IsNullOrEmpty(unprotectedToken))
                {
                    return true;
                }

                if (!DateTime.TryParse(unprotectedToken, CultureInfo.CurrentCulture, out var expiration)
                    || DateTime.UtcNow > expiration)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate token");
                return true;
            }
        }
    }
}
