using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [Route("api/confirm")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        protected readonly DbApp db;

        public ConfirmController(DbApp db)
        {
            this.db = db;
        }

        [HttpGet("email/{code}")]
        public async Task<IActionResult> ConfirmEmail(string code, [FromQuery] string e)
        {
            User? user = await db.Users.FirstOrDefaultAsync(p => p.ActivationCode == code && p.Email == e);
            if (user == null)
            {
                return BadRequest(new BaseResponse.ErrorResponse("User not found"));
            }
            user.IsActivated = true;
            user.ActivationCode = null;
            await db.SaveChangesAsync();
            return Redirect("/login");
        }
    }
}
