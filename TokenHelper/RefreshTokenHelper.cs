using ConverterRestApi.Data;
using ConverterRestApi.Helper;
using ConverterRestApi.Model;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace ConverterRestApi.TokenHelper
{
    public class RefreshTokenHelper : IRefreshToken
    {
        private readonly ConverterRestApiContext _context;
        
        public RefreshTokenHelper()
        {
        }

        public RefreshTokenHelper(ConverterRestApiContext context)
        {
            _context = context;
        }
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

        public string RefreshTokenGenerator(CredentialsParameters creds, LoginParameters userCred)
        {
            var (refreshToken, expiredTime) = GenerateToken();

            var users = _context.RefreshToken.FirstOrDefault(cred => cred.UserId == creds.UserName);
            if (users != null)
            {
                users.RefreshToken = refreshToken;
                users.ExpirationTime = expiredTime;
                _context.SaveChanges();
            }
            else
            {
                var newRefreshToken = new RefreshTokenParameters()
                {
                    UserId = userCred.UserName.ToLower(),
                    Password = userCred.Password.ToLower(),
                    Phone = creds.Phone,
                    Email = creds.Email.ToLower(),
                    TokenId = new Random().Next().ToString(),
                    RefreshToken = refreshToken,
                    IsActive = true,
                    ExpirationTime = expiredTime

                };
                _context.RefreshToken.Add(newRefreshToken);
                _context.SaveChanges();
            }

            return refreshToken;
        }

        public bool ValidateRefreshToken(string userId, string userPhone)
        {
            
            string refreshToken = _context.RefreshToken.FirstOrDefault(refresh => refresh.UserId == userId && refresh.Phone == userPhone).RefreshToken;
            if (refreshToken != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Test(string? accessToken, string? username, string? phone)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
            string refreshToken = _context.RefreshToken.FirstOrDefault(refresh => refresh.UserId == username && refresh.Phone == phone).RefreshToken;
            if (refreshToken != null)
            {
                return refreshToken;
            }
            else
            {
                return "";
            }
        }
    }
}
