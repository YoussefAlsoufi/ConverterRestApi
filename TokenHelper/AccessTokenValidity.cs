using ConverterRestApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConverterRestApi.TokenHelper
{
    public class AccessTokenValidity
    {
        //public static ClaimsPrincipal ValidateAccessToken(string jwtToken,TokenValidationParameters tokenParameters)
        //{
        //    var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, tokenParameters, out _);

        //    return principal;

        //}
        public static bool ValidateAccessToken(string jwtToken, TokenValidationParameters tokenParameters)
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, tokenParameters, out _);

            return true;

        }

    }
}
