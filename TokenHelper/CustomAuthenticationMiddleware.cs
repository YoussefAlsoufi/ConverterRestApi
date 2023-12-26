using ConverterRestApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterRestApi.TokenHelper
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ConverterRestApiContext _context;
        private string? AccessTokenExtracted;
        public CustomAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ConverterRestApiContext context)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Invoke(HttpContext context)
        {
            AccessTokenExtracted = ExtractTokenFromRequest(context.Request);
            AccessTokenHelper accessTokenHelper = new(_configuration);

            if (!string.IsNullOrEmpty(AccessTokenExtracted) && !accessTokenHelper.IsTokenValid(AccessTokenExtracted))
            {
                var newAccessToken = await RefreshAccessTokenAsync(context);

                // Update the request with the new access token
                context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";
            }

            await _next.Invoke(context);
        }

        private static string? ExtractTokenFromRequest(HttpRequest request)
        {
            // Check if the Authorization header is present in the request
            if (request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
            {
                // Get the first Authorization header value
                var authorizationHeaderValue = authorizationHeaderValues.FirstOrDefault();

                // Check if the Authorization header value is not null or empty
                if (!string.IsNullOrEmpty(authorizationHeaderValue) && authorizationHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the token from the Authorization header
                    return authorizationHeaderValue["Bearer ".Length..];
                }
            }

            // If the Authorization header is not present or doesn't follow the expected format, return null
            return null;
        }
        private async Task<string> RefreshAccessTokenAsync(HttpContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            RefreshTokenHelper refreshTokenHelper = new(_context);
            string newAccessToken = null;

            if (tokenHandler.ReadToken(AccessTokenExtracted) is JwtSecurityToken claimsPrincipal)
            {
                // Access the "UserName" claim
                var usernameClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserName");
                var phoneClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Phone");
                var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Email");

                if (usernameClaim != null && phoneClaim != null && emailClaim != null)
                {
                    var username = usernameClaim.Value;
                    var phone = phoneClaim.Value;
                    var email = emailClaim.Value;

                    var creds = _context.Credentials.FirstOrDefault(i => i.UserName == username && i.Email == email && i.Phone == phone);

                    if (creds != null && refreshTokenHelper.ValidateRefreshToken(creds))
                    {
                        var refreshTokenEntity = _context.RefreshToken.FirstOrDefault(refresh => refresh.UserId == creds.UserName && refresh.Phone == creds.Phone && refresh.Email == creds.Email);

                        if (refreshTokenEntity != null)
                        {
                            var accessToken = AccessTokenExtracted;
                            var refreshToken = refreshTokenEntity.RefreshToken;

                            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                            {
                                newAccessToken = await CallRefreshTokenEndpoint(accessToken, refreshToken);
                            }
                        }
                    }
                }
            }

            return newAccessToken ?? ""; 
        }
        private static async Task<string> CallRefreshTokenEndpoint(string accessToken, string refreshToken)
        {
            using var httpClient = new HttpClient();

            var requestData = new
            {
                jwtToken = accessToken,
                refreshToken = refreshToken
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var refreshTokenEndpoint = "https://localhost:7020/api/Credentials/Refresh";

            try
            {
                var refreshTokenResponse = await httpClient.PostAsync(refreshTokenEndpoint, content);

                // Check if the request was successful (status code 2xx)
                if (refreshTokenResponse.IsSuccessStatusCode)
                {
                    // Read the new access token from the response
                    string newAccessToken = await refreshTokenResponse.Content.ReadAsStringAsync();
                    return newAccessToken;
                }
                else
                {
                    // Handle the case where the refresh token request failed
                    // Extract and log response content for better error handling
                    var responseContent = await refreshTokenResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Refresh token request failed. Status code: {refreshTokenResponse.StatusCode}, Content: {responseContent}");
                    throw new HttpRequestException($"Refresh token request failed. Status code: {refreshTokenResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions (e.g., network issues) here
                Console.WriteLine($"An error occurred while refreshing the token: {ex.Message}");
                throw;
            }
        }


    }
}
