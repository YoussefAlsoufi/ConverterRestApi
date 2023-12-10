using System.Security.Cryptography;

namespace ConverterRestApi
{
    public class RefreshTokenGenerator : IRefreshToken
    {
        public (string RefreshToken, DateTime ExpirationTime) GenerateToken()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            string RefreshToken = Convert.ToBase64String(randomNumber);

            // Set the expiration time for the associated access token (e.g., 30 days from now)
            DateTime expirationTime = DateTime.UtcNow.Add(TimeSpan.FromDays(30));

            return (RefreshToken, expirationTime);
        }
    }
}
