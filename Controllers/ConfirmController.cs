using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Responses;
using System.Text.Json;

namespace ShineSyncControl.Controllers
{
    [Route("api/confirm")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        protected readonly DbApp db;
        protected readonly IDistributedCache cache;

        public ConfirmController(DbApp db, IDistributedCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        [HttpGet("email/{code}")]
        public async Task<IActionResult> ConfirmEmail(string code, [FromQuery] string e)
        {
            string? cachedUser = await cache.GetStringAsync($"user.activation-code.{code}");
            if (cachedUser == null)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Invalid activation code"));
            }

            User? user = JsonSerializer.Deserialize<User>(cachedUser);
            if (user == null || user.Email != e)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Invalid activation code"));
            }


            user.IsActivated = true;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            await cache.RemoveAsync($"user.activation-code.{code}");

            return Redirect("/login");
        }
    }
}
