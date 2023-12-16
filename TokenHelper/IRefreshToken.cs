using ConverterRestApi.Model;

namespace ConverterRestApi.TokenHelper
{
    public interface IRefreshToken
    {
        (string RefreshToken, DateTime ExpirationTime) GenerateToken();
        string RefreshTokenGenerator(CredentialsParameters creds, LoginParameters userCred);
    }
}
