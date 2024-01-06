using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.Email;
using ShineSyncControl.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ShineSyncControl.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        protected readonly IEmailService email;
        protected readonly IConfiguration _configuration;
        protected readonly IDistributedCache cache;

        public AuthController(DbApp db, IEmailService email, IConfiguration configuration, IDistributedCache cache) : base(db)
        {
            this.email = email;
            _configuration = configuration;
            this.cache = cache;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            User? user = await DB.Users.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (user != null)
            {
                return BadRequest(new BaseResponse.ErrorResponse("User already exists"));
            }
            string passwordHash = PasswordHasher.Hash(request.Password);
            string activationCode = Guid.NewGuid().ToString();
            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = passwordHash,
                IsActivated = false,
                Role = await DB.Roles.Where(p => p.Role == UserRoles.User).SingleAsync(),
            };
            await cache.SetStringAsync($"user.activation-code.{activationCode}", JsonSerializer.Serialize(user), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });

            await email.SendEmailUseTemplateAsync(user.Email, "ActivateAccount.html", new Dictionary<string, string>
            {
                {"firstName", user.FirstName },
                {"link", $"https://{HttpContext.Request.Host}/api/confirm/email/{activationCode}?e={Uri.UnescapeDataString(user.Email)}" }
            });

            return Ok(new BaseResponse.SuccessResponse());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, [FromQuery] bool isJWT = false)
        {

            User? user = await DB.Users.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (user == null)
            {
                return BadRequest(new AuthorizationFailedResponse());
            }

            if (!PasswordHasher.Validate(request.Password, user.Password))
            {
                return BadRequest(new AuthorizationFailedResponse());
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["JWT:ExpirationHours"])),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            if(isJWT)
            {
                return Ok(new BaseResponse.SuccessResponse(null, tokenString));
            }
            else
            {
                HttpContext.Response.Cookies.Append(".VoDACode.Authorize", tokenString);
                return Ok(new BaseResponse.SuccessResponse());
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete(".VoDACode.Authorize");
            return Ok(new BaseResponse.SuccessResponse());
        }

        [AuthorizeAnyType]
        [HttpGet("check")]
        public IActionResult Check()
        {
            if (HttpContext.User.Claims
                    .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value is null)
            {
                return Ok(new BaseResponse.ErrorResponse());
            }
            return Ok(new BaseResponse.SuccessResponse());
        }
    }
}
