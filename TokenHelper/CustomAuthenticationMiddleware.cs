using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConverterRestApi.TokenHelper
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private string? AccessTokenExtracted;
        public CustomAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Invoke(HttpContext context)
        {
            AccessTokenExtracted = ExtractTokenFromRequest(context.Request);
            AccessTokenHelper accessTokenHelper = new(_configuration);

            if (!string.IsNullOrEmpty(AccessTokenExtracted) && accessTokenHelper.IsTokenInvalidOrExpired(AccessTokenExtracted))
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
                    return authorizationHeaderValue.Substring("Bearer ".Length);
                }
            }

            // If the Authorization header is not present or doesn't follow the expected format, return null
            return null;
        }

        private async Task<string> RefreshAccessTokenAsync(HttpContext context)
        {


            return "youssef alsoufi";
        }
    }
}
