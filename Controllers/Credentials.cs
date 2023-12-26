using ConverterRestApi.Helper;
using Microsoft.AspNetCore.Mvc;
using ConverterRestApi.Model;
using ConverterRestApi.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using ConverterRestApi.TokenHelper;
using Microsoft.IdentityModel.Tokens;

namespace ConverterRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class Credentials : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string Client = "user";
        private readonly string Admin = "admin";
        public Credentials(ConverterRestApiContext context, IConfiguration configuration)
        {
            _configuration = configuration;
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
                Role = userCred.Role == "string" ? Client : Admin
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
            AccessTokenHelper accessToken = new(_configuration);
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
                    (tokenValidationParameters, jwtToken) = accessToken.GenerateAccessToken( creds, 43200);
                    
                }
                else
                {
                    (tokenValidationParameters, jwtToken) = accessToken.GenerateAccessToken( creds, 1);

                    // Generate a Refresh Token:
                    IRefreshToken refreshTokenGenerator = new RefreshTokenHelper(_context);
                    responseToken.RefreshToken = refreshTokenGenerator.RefreshTokenGenerator(creds, userCred);
                }
                responseToken.JwtToken = jwtToken;
                try
                {

                    //accessToken.ValidateAccessToken(responseToken.JwtToken, tokenValidationParameters, HttpContext);
                    // Token is valid
                    Console.WriteLine("Valid");
                    return Ok(responseToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid with Exception: {0}", ex);
                    // Invalid token
                    return BadRequest("InValid Token");
                }


            }
        }

        [HttpPost("Refresh")]
        [AllowAnonymous]
        public IActionResult RefershToken([FromBody] ResponseToken tokenParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            AccessTokenHelper accessToken = new(_configuration);
            RefreshTokenHelper refreshHelper = new(_context);
            //string jwtToken;

            (TokenValidationParameters tokenValidationParameters, string jwtToken) = (null, null);

            if (tokenHandler.ReadToken(tokenParameters.JwtToken) is JwtSecurityToken claimsPrincipal)
            {
                // Access the "UserName" claim
                var usernameClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserName");
                string username = usernameClaim?.Value ?? "No UserName claim found";

                // Access the "Phone" claim
                var phoneClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Phone");
                string phone = phoneClaim?.Value ?? "No Phone claim found";

                // Access the "Email" claim
                var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Email");
                string email = emailClaim?.Value ?? "No Email claim found";

                var creds = _context.Credentials.FirstOrDefault(i => i.UserName == username && i.Email == email && i.Phone == phone);

                if (creds != null && refreshHelper.ValidateRefreshToken(creds))
                {
                    (tokenValidationParameters, jwtToken) = accessToken.GenerateAccessToken(creds, 1);
                    tokenParameters.JwtToken = jwtToken;
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }

            return Ok(tokenParameters.JwtToken);
        }

    }
    
}
