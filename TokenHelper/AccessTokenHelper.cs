using ConverterRestApi.Migrations;
using ConverterRestApi.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConverterRestApi.TokenHelper
{
    public class AccessTokenHelper : IJwtTokenServices
    {
        private readonly IConfiguration _configuration;
        private List<Claim> claims;

        public AccessTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (TokenValidationParameters, string) GenerateAccessToken(LoginParameters userCred, CredentialsParameters creds, int ExpirationTime)
        {
            var authKey = _configuration.GetValue<string>("JWTSettings:SecretKey");
            var audience = _configuration.GetValue<string>("JWTSettings:Audience");
            var issuer = _configuration.GetValue<string>("JWTSettings:Issuer");
            var subject = _configuration.GetValue<string>("JWTSettings:Subject");
            claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("UserName", userCred.UserName),
                    new Claim(ClaimTypes.Role, "user")
                };
            if (creds.Role == "admin")
            {
                claims.Add( new Claim(ClaimTypes.Role, "admin"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(ExpirationTime),
                signingCredentials: signIn
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);


            var tokenParameters = new TokenParameters(_configuration).GenerateTokenParameters(); 

            return (tokenParameters, jwtToken);

        }

        public string ValidateAccessToken(string jwtToken, TokenValidationParameters tokenParameters, HttpContext httpContext)
        {
            string authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                // Authorization header is missing
                return ("Authorization header is missing.");
            }

            // Check if the Authorization header has the Bearer scheme
            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Invalid or unsupported authorization scheme
                return ("Invalid or unsupported authorization scheme.");
            }

            // Extract the token from the Authorization header
            string token = authorizationHeader["Bearer ".Length..].Trim();
            var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, tokenParameters, out _);

            return "true";
        }

    }
}
