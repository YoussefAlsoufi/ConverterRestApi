using ConverterRestApi.Model;
using Microsoft.IdentityModel.Tokens;

namespace ConverterRestApi.TokenHelper
{
    public interface IJwtTokenServices
    {
        (TokenValidationParameters, string) GenerateAccessToken(CredentialsParameters creds, int ExpirationTime);
        //bool ValidateAccessToken(TokenValidationParameters tokenParameters, HttpContext httpContext);
    }
}
