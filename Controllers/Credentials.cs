using ConverterRestApi.Helper;
using Microsoft.AspNetCore.Mvc;
using ConverterRestApi.Model;
using ConverterRestApi.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using ConverterRestApi.TokenHelper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace ConverterRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class Credentials : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly string Client = "user";
        private readonly string Admin = "admin";
        public Credentials (ConverterRestApiContext context, IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
        {
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            _context = context;

            //credentials = option.Value; // here using IOPtions interface has an advantage: Instead it was possible to use directly JWTSettings _jwtSettings and then
                                        // _jwtSettings.SecretKey , When you directly inject MySettings without using IOptions<T>, the settings are bound and resolved at the time the Credentials class instance is created.
                                        // This means that any changes made to the configuration during the application's runtime won't be reflected in the MyClass instance, as it holds the initial values.However,
                                        // when using IOptions<T>, the configuration settings are accessed via the IOptions < T > interface. The settings can be refreshed and updated during runtime without needing to recreate the Credentials instance.
                                        // This provides a more dynamic approach to configuration changes and allows for runtime updates without restarting the application.
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] CredentialsParameters userCred)
        {
            if (userCred.Email.Equals("string") || !CheckInputsValidity.IsValidEmail(userCred.Email))
            {
                return Ok("Enter a Valid Email");

            }
            if (userCred.UserName.Equals("string") || !CheckInputsValidity.IsValidUserName(userCred.UserName))
            {
                return Ok("Enter a Valid UserName!");
            }
            if (userCred.Phone.Equals("string") || !CheckInputsValidity.IsValidPhone(userCred.Phone))
            {
                return Ok("Enter a Valid Phone Number!");
            }
            if (userCred.Password.Equals("string") || !CheckInputsValidity.IsPasswordValid(userCred.Password))
            {
                return Ok("Password should contains  Upper,Lower,Digits,characters.");
            }

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
            TokenValidationParameters tokenValidationParameters = new();
            string jwtToken;
            ResponseToken responseToken = new();
            AccessTokenHelper accessToken = new AccessTokenHelper(_configuration);
            var creds = _context.Credentials.FirstOrDefault(i => i.UserName == userCred.UserName.ToLower() || i.Email == userCred.UserName.ToLower() || i.Phone == userCred.UserName.ToLower() 
            && i.Password == EncryptCredentials.EncryptPassword(userCred.Password));
            if (creds == null)
            {
                return Unauthorized();
            }
            else
            {
                // Generate Access Token:
                if (creds.Role == "admin")
                {
                    (tokenValidationParameters, jwtToken) = accessToken.GenerateAccesstoken(userCred, 43200);
                }
                else
                {
                    (tokenValidationParameters, jwtToken) = accessToken.GenerateAccesstoken(userCred, 1);
                    responseToken.JwtToken = jwtToken;

                    // Generate a Refresh Token:
                    IRefreshToken refreshTokenGenerator = new RefreshTokenHelper(_context);
                    responseToken.RefreshToken = refreshTokenGenerator.RefreshTokenGenerator(creds, userCred);
                }

                try
                {
                    AccessTokenValidity.ValidateAccessToken(responseToken.JwtToken, tokenValidationParameters);
                    //var principal =  AccessTokenValidity.ValidateAccessToken(responseToken.JwtToken, tokenValidationParameters);
                    // Token is valid
                    Console.WriteLine("Valid");
                    return Ok(responseToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid with Exception: {0}",ex);
                    // Invalid token
                    return BadRequest("InValid Token");
                }


            }
        }
        //public bool IsRefreshTokenExpired(DateTime expirationTime)
        //{
        //    // Compare the current time with the expiration time
        //    return DateTime.UtcNow >= expirationTime;
        //}


    }
}
