using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using app;
using Microsoft.Extensions.Configuration;
using System.Text;
using app.Models;
using app.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    public class LoginCredentials
    {
      public string Email { get; set;}
      public string Password { get; set;}
    }

    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly SigningCredentials SubscriberSigningCreds;
        private readonly SigningCredentials ProviderSigningCreds;
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsController> _logger;
        private readonly UsersDBContext _dbContext;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

        public  AccountController(IHubContext<NotificationHub, INotificationHub> notificationHub, IConfiguration configuration,
           UsersDBContext dbContext, ILogger<PaymentsController> logger)
        {
            this._hubContext = notificationHub;
            this._configuration = configuration;
            this._dbContext = dbContext;
            this._logger = logger;

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
            SubscriberSigningCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            ProviderSigningCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
        }



        [HttpPost("login")]
        public async Task<IActionResult> LoginSubscriber([FromBody]LoginCredentials creds)
        {

            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }

            var principal = GetPrincipal(creds, Startup.JWTAuthScheme);

            await HttpContext.SignInAsync(Startup.JWTAuthScheme, principal);

            return Json(new
            {
                name = principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role),
                id = Guid.NewGuid().ToString()
            });
        }
        [HttpPost("loginprovider")]
        public async Task<IActionResult> LoginProvider([FromBody]LoginCredentials creds)
        {
            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }

            var principal = GetPrincipal(creds, Startup.JWTAuthScheme);

            await HttpContext.SignInAsync(Startup.JWTAuthScheme, principal);


            return Json(new
            {
                name = principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role),
                roomId = Guid.NewGuid()
            });

        }

  

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return StatusCode(200);
        }

        [HttpGet("context")]
        public JsonResult Context()
        {
          return Json(new
          {
              name = this.User?.Identity?.Name,
              email = this.User?.FindFirstValue(ClaimTypes.Email
              ),
              role = this.User?.FindFirstValue(ClaimTypes.Role),
          });
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody]LoginCredentials creds)
        {
            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }
            var subscriber = _dbContext.Subscribers.FirstOrDefault(c => c.UserName == creds.Email);

            ClaimsPrincipal principal;
            JwtSecurityToken token;
            GetToken(this.SubscriberSigningCreds, creds, out principal, out token);

            return Json(new
            {
                jwtToken = _tokenHandler.WriteToken(token),
                name = subscriber.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role),
                clientId = subscriber.Id.ToString()
            });
        }


        /// <summary>
        /// Used by provider to log into system
        /// </summary>
        /// <param name="creds"></param>
        /// <returns></returns>
        [HttpPost("join")]
        public  IActionResult Join([FromBody]LoginCredentials creds)
        {
            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }

            ClaimsPrincipal principal;
            JwtSecurityToken token;
            GetToken(ProviderSigningCreds, creds, out principal, out token);

            var provider = _dbContext.Providers.FirstOrDefault(c => c.UserName == creds.Email);

            var roomId = GetRandomString();
         

            return Json(new
            {
                jwtToken = _tokenHandler.WriteToken(token),
                name = string.Format("{0} {1}", provider.FirstName, provider.LastName),     //  principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role),
                roomId = roomId,
                id = provider.Id.ToString(),
                avatar = provider.PhotoName_URL
            });
        }

        private void GetToken(SigningCredentials signingCreds,   LoginCredentials creds, out ClaimsPrincipal principal, out JwtSecurityToken token)
        {
            principal = GetPrincipal(creds, Startup.JWTAuthScheme);
            var issuer = _configuration["TokenAuthentication:Issuer"];
            var audience = _configuration["TokenAuthentication:Audience"];
            token = new JwtSecurityToken(
                issuer,
                audience,
                principal.Claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: signingCreds);
        }

        private string GetRandomString()
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
           return str_build.ToString();
        }
        private bool ValidateLogin(LoginCredentials creds)
        {
            // On a real project, you would use a SignInManager<ApplicationUser> to verify the identity
            // using:
            //  _signInManager.PasswordSignInAsync(user, password, lockoutOnFailure: false);
            // When using JWT you would rather
            //  _signInManager.UserManager.FindByEmailAsync(email);
            //  _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            // For this sample, all logins are successful!
            return true;
        }

        private ClaimsPrincipal GetPrincipal(LoginCredentials creds, string authScheme)
        {
          // On a real project, you would use the SignInManager<ApplicationUser> to locate the user by its email
          // and to build its ClaimsPrincipal using:
          //  var user = await _signInManager.UserManager.FindByEmailAsync(email);
          //  var principal = await _signInManager.CreateUserPrincipalAsync(user)

          // Here we are just creating a Principal for any user, using its email and a hardcoded "User" role
          var claims = new List<Claim>
          {
              new Claim(ClaimTypes.Name, creds.Email),
              new Claim(ClaimTypes.Email, creds.Email),
              new Claim(ClaimTypes.Role, "User"),
          };
          return new ClaimsPrincipal(new ClaimsIdentity(claims, authScheme));
        }
    }
}