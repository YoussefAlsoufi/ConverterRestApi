﻿using ConverterRestApi.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConverterRestApi.TokenHelper
{
    public class AccessTokenHelper
    {
        private readonly IConfiguration _configuration;
        public AccessTokenHelper(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public (TokenValidationParameters,string) GenerateAccesstoken(LoginParameters userCred)
        {
            var authKey = _configuration.GetValue<string>("JWTSettings:SecretKey");
            var audience = _configuration.GetValue<string>("JWTSettings:Audience");
            var issuer = _configuration.GetValue<string>("JWTSettings:Issuer");
            var subject = _configuration.GetValue<string>("JWTSettings:Subject");

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("UserName", userCred.UserName)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: signIn
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);

            var tokenParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["JWTSettings:Audience"],
                ValidIssuer = _configuration["JWTSettings:Issuer"],
                ClockSkew = TimeSpan.FromMinutes(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey))

            };

            return (tokenParameters, jwtToken);
        }
    }
}