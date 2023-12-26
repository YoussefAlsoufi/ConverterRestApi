using ConverterRestApi.Migrations;
using ConverterRestApi.Model;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConverterRestApi.TokenHelper
{
    public class AccessTokenHelper : IJwtTokenServices
    {
        private readonly IConfiguration _configuration;
        private List<Claim>? claims;
        public AccessTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (TokenValidationParameters, string) GenerateAccessToken(CredentialsParameters creds, int ExpirationTime)
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
                new Claim("UserName", creds.UserName),
                new Claim("Email", creds.Email),
                new Claim("Phone", creds.Phone),
                new Claim(ClaimTypes.Role, creds.Role)
            };

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
        public bool IsTokenValid(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                // Log or handle the case where the token is null or empty
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration.GetValue<string>("JWTSettings:Issuer"),
                ValidAudience = _configuration.GetValue<string>("JWTSettings:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWTSettings:SecretKey")))
            };

            try
            {
                // Log the token before validation
                Console.WriteLine($"Token before validation: {accessToken}");

                // Validate the token and capture the validated token
                tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);

                // Log the properties of the validated token
                Console.WriteLine($"ValidTo: {validatedToken.ValidTo}");

                // If the validation passed, check the token's expiration separately
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    // Log the case where the token is expired
                    Console.WriteLine("Token is expired");
                    return false;
                }

                return true; // Token is valid
            }
            catch (SecurityTokenException ex)
            {
                // Log the exception and return false
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
