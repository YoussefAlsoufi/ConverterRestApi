using ConverterRestApi.Model;
using Microsoft.IdentityModel.Tokens;

namespace ConverterRestApi.TokenHelper
{
    public interface IJwtTokenServices
    {
        (TokenValidationParameters, string) GenerateAccessToken(LoginParameters userCred, int ExpirationTime);
        string ValidateAccessToken(string jwtToken, TokenValidationParameters tokenParameters, HttpContext httpContext);
    }
}
