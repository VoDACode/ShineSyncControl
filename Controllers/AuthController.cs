using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShineSyncControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly DbApp db;

        public AuthController(DbApp db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(db.Users);
        }
    }
}
