using ConverterRestApi.Data;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ConverterRestApi.TokenHelper
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConverterRestApiContext _context;
        private readonly IConfiguration _configuration;

        public TokenRefreshMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, ConverterRestApiContext context, IConfiguration configuration)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var newAccessToken = new AccessTokenHelper(_configuration);
            // Check if the request includes an access token
            string? accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshTokenValidation = new RefreshTokenHelper(_context);

            var tokenHandler = new JwtSecurityTokenHandler();

            //var usernameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            if (!string.IsNullOrEmpty(accessToken))
            {
                JwtSecurityToken? jsonToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
                string? usernameClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                string? phoneClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "phone")?.Value;
                // Check if the access token is expired
                if (IsTokenExpired(accessToken))
                {
                    // Use refresh token to obtain a new access token
                    var refreshToken = refreshTokenValidation.Test(accessToken, usernameClaim,phoneClaim); // Implement this method based on your storage mechanism

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        Console.WriteLine("Create a new access token here!");
                    }
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }

        private static bool IsTokenExpired(string accessToken)
        {
            try
            {
                // Decode the JWT token to get the expiration claim
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    // Token is not a valid JWT
                    return true;
                }

                // Check the expiration claim
                var expiration = jsonToken.ValidTo.AddMinutes(-1);

                return expiration <= DateTime.UtcNow;
            }
            catch (Exception)
            {
                // Exception handling, e.g., if the token cannot be decoded
                return true;
            }
        }

        private void UpdateAccessTokenInRequest(HttpContext context, string newAccessToken)
        {
            // Update the Authorization header with the new access token
            context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";
        }

    }

}
