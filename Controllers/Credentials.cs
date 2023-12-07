﻿using ConverterRestApi.Helper;
using Microsoft.AspNetCore.Mvc;
using ConverterRestApi.Model;
using ConverterRestApi.Data;
using Microsoft.Extensions.Options;
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
        public static string userToken;
        private readonly ConverterRestApiContext _context;
        private readonly JwtSettings credentials;
        private readonly IConfiguration _configuration;
        private readonly string Client = "user";
        private readonly string Admin = "admin";
        public Credentials (ConverterRestApiContext context, IOptions<JwtSettings> option, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            credentials = option.Value; // here using IOPtions interface has an advantage: Instead it was possible to use directly JWTSettings _jwtSettings and then
                                        // _jwtSettings.SecretKey , When you directly inject MySettings without using IOptions<T>, the settings are bound and resolved at the time the Credentials class instance is created.
                                        // This means that any changes made to the configuration during the application's runtime won't be reflected in the MyClass instance, as it holds the initial values.However,
                                        // when using IOptions<T>, the configuration settings are accessed via the IOptions < T > interface. The settings can be refreshed and updated during runtime without needing to recreate the Credentials instance.
                                        // This provides a more dynamic approach to configuration changes and allows for runtime updates without restarting the application.
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] CredentialsParameters userCred)
        {
            var encryptedPassword = EncryptCredentials.EncryptPassword(userCred.Password);
            var creds = new CredentialsParameters
            {
                UserName = userCred.UserName.ToLower(),
                Password = encryptedPassword,
                Email = userCred.Email.ToLower(),
                Phone = userCred.Phone,
                Role = userCred.Role == "string" ? Client: Admin
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var existingUser = await _context.Credentials.FirstOrDefaultAsync(u => u.Email == userCred.Email.ToLower() && u.Phone == userCred.Phone.ToLower());
                if (existingUser == null)
                {
                    if (!CheckInputsValidity.IsValidEmail(userCred.Email))
                    {
                        return Ok("Enter a Valid Email");
  
                    }
                    if (!CheckInputsValidity.IsValidUserName(userCred.UserName))
                    {
                        return Ok("Enter a Valid UserName!");
                    }
                    if (!CheckInputsValidity.IsValidPhone(userCred.Phone))
                    {
                        return Ok("Enter a Valid Phone Number!");
                    }
                    _context.Credentials.Add(creds);
                    await _context.SaveChangesAsync();

                    return Ok("SignUp done");
                }
                else
                {
                    return Ok("The Email/Phone are already exist.");
                }

            }

        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult CheckLogin([FromBody] LoginParameters userCred)
        {
            var creds = _context.Credentials.FirstOrDefault(i => i.UserName == userCred.UserName.ToLower() || i.Email == userCred.UserName.ToLower() || i.Phone == userCred.UserName.ToLower() 
            && i.Password == userCred.Password);
            if (creds == null)
            {
                return Unauthorized();
            }
            // generate a token:
            else
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
                    expires: DateTime.UtcNow.AddHours(20),
                    signingCredentials: signIn
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                userToken = tokenHandler.WriteToken(token);

                var tokenParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = _configuration["JWTSettings:Audience"],
                    ValidIssuer = _configuration["JWTSettings:Issuer"],
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey))

                };
                try
                {
                    var principal = new JwtSecurityTokenHandler().ValidateToken(userToken, tokenParameters, out _);
                    // Token is valid
                    Console.WriteLine("Valid");
                    return Ok(userToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid");
                    // Invalid token
                    return BadRequest("InValid Token");
                }


                //return Ok(userToken);


            }
        }


    }
}
