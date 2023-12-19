using ConverterRestApi.Migrations;
using ConverterRestApi.Model;
using Microsoft.AspNetCore.Http;
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
                new Claim(ClaimTypes.Name, creds.UserName),
                new Claim(ClaimTypes.Email, creds.Email),
                new Claim(ClaimTypes.MobilePhone, creds.Phone),
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

        //public bool ValidateAccessToken(TokenValidationParameters tokenParameters, HttpContext httpContext)
        //{

        //    var token = ExtractTokenfromHeader(httpContext);
        //    try
        //    {
        //        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenParameters, out _);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Invalid with Exception: {0}", ex);
        //        // Invalid token
        //        return false;
        //    }
 
        //}
        //public string ExtractTokenfromHeader(HttpContext httpContext)
        //{
        //    string authorizationHeader = httpContext.Request.Headers["Authorization"];

        //    if (string.IsNullOrWhiteSpace(authorizationHeader))
        //    {
        //        // Authorization header is missing
        //        return ("Authorization header is missing.");
        //    }

        //    // Check if the Authorization header has the Bearer scheme
        //    if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Invalid or unsupported authorization scheme
        //        return ("Invalid or unsupported authorization scheme.");
        //    }


        //    // Extract the token from the Authorization header
        //    return authorizationHeader["Bearer ".Length..].Trim();
        //}

    }
}
