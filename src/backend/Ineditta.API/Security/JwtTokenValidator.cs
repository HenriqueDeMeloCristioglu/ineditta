using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

namespace Ineditta.API.Security
{
    public class JwtTokenValidator
    {
        public bool ValidateJwtToken(string jwtToken, string authority, string audience, string publicKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Convert the public key to an RSA key
            var rsaKey = new RsaSecurityKey(new System.Security.Cryptography.RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(publicKey), // Convert from Base64URL to byte array
                Exponent = Base64UrlEncoder.DecodeBytes("AQAB"),   // Use a standard exponent value for RSA keys
            });

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = authority,
                ValidAudience = audience,
                IssuerSigningKey = rsaKey
            };

            try
            {
                _ = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);

                return true;
            }
            catch (SecurityTokenValidationException)
            {
                return false;
            }
        }
    }
}