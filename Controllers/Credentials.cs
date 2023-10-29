using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConverterRestApi.Model;
using ConverterRestApi.Data;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ConverterRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class Credentials : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly JwtSettings credentials;
        public Credentials (ConverterRestApiContext context, IOptions<JwtSettings> option)
        {
            _context = context;
            credentials = option.Value; // here using IOPtions interface has an advantage: Instead it was posible to use directly JWTSettings _jwtSettings and then
                                        // _jwtSettings.SecretKey , When you directly inject MySettings without using IOptions<T>, the settings are bound and resolved at the time the Credentials class instance is created.
                                        // This means that any changes made to the configuration during the application's runtime won't be reflected in the MyClass instance, as it holds the initial values.However,
                                        // when using IOptions<T>, the configuration settings are accessed via the IOptions < T > interface. The settings can be refreshed and updated during runtime without needing to recreate the Credentials instance.
                                        // This provides a more dynamic approach to configuration changes and allows for runtime updates without restarting the application.
        }
       
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] CredentialsParameters userCred)
        {
            var creds = _context.Credentials.FirstOrDefault(i => i.UserName == userCred.UserName && i.Password == userCred.Password);
            if (creds == null)
            {
                return Unauthorized();
            }
            // generate a token:
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(credentials.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, creds.UserName),
                    }),
                Expires= DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string finalToken= tokenHandler.WriteToken(token);

            return Ok(finalToken);
        }
        
    }
}
