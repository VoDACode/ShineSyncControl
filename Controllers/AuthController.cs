using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.Email;
using ShineSyncControl.Tools;
using System.Security.Claims;

namespace ShineSyncControl.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly DbApp db;
        protected readonly IEmailService email;

        public AuthController(DbApp db, IEmailService email)
        {
            this.db = db;
            this.email = email;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            User? user = await db.Users.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (user != null)
            {
                return BadRequest(new BaseResponse.ErrorResponse("User already exists"));
            }
            string passwordHash = PasswordHasher.Hash(request.Password);
            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = passwordHash,
                ActivationCode = Guid.NewGuid().ToString(),
                IsActivated = false
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            await email.SendEmailUseTemplateAsync(user.Email, "ActivateAccount.html", new Dictionary<string, string>
            {
                {"firstName", user.FirstName },
                {"link", $"https://{HttpContext.Request.Host}/api/confirm/email/{user.ActivationCode}?e={Uri.UnescapeDataString(user.Email)}" }
            });

            return Ok(new BaseResponse.SuccessResponse());
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {

            User? user = await db.Users.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (user == null)
            {
                return BadRequest(new AuthorizationFailedResponse());
            }

            string passwordHash = PasswordHasher.Hash(request.Password);
            if (user.Password != passwordHash)
            {
                return BadRequest(new AuthorizationFailedResponse());
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Ok(new BaseResponse.SuccessResponse());
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new BaseResponse.SuccessResponse());
        }
    }
}
