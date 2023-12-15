using ConverterRestApi.Model;

namespace ConverterRestApi
{
    public interface IRefreshToken
    {
        (string RefreshToken, DateTime ExpirationTime) GenerateToken ();
        string RefreshTokenGenerator(CredentialsParameters creds, LoginParameters userCred);
    }
}
