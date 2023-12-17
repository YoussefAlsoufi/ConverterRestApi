using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConverterRestApi.TokenHelper
{
    public class TokenParameters
    {
        private readonly IConfiguration _configuration;
        public TokenParameters(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenValidationParameters GenerateTokenParameters()
        {
            var authKey = _configuration.GetValue<string>("JWTSettings:SecretKey");
            var tokenParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["JWTSettings:Audience"],
                ValidIssuer = _configuration["JWTSettings:Issuer"],
                ClockSkew = TimeSpan.FromMinutes(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey))

            };

            return tokenParameters;
        }
    }
}
