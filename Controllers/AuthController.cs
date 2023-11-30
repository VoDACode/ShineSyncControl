using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.Email;
using ShineSyncControl.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShineSyncControl.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        protected readonly IEmailService email;
        protected readonly IConfiguration _configuration;

        public AuthController(DbApp db, IEmailService email, IConfiguration configuration) : base(db)
        {
            this.email = email;
            _configuration = configuration;
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
            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = passwordHash,
                ActivationCode = Guid.NewGuid().ToString(),
                IsActivated = false
            };
            await DB.Users.AddAsync(user);
            await DB.SaveChangesAsync();

            await email.SendEmailUseTemplateAsync(user.Email, "ActivateAccount.html", new Dictionary<string, string>
            {
                {"firstName", user.FirstName },
                {"link", $"https://{HttpContext.Request.Host}/api/confirm/email/{user.ActivationCode}?e={Uri.UnescapeDataString(user.Email)}" }
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

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"])),
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new BaseResponse.SuccessResponse());
        }

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
