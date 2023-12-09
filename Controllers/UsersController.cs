using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseController
    {
        public UsersController(DbApp db) : base(db)
        {
        }

        [AuthorizeAnyType(Type = AuthorizeType.Cookie | AuthorizeType.JWT)]
        [HttpGet("self")]
        public IActionResult GetSelf()
        {
            return Ok(new UserInfoResponse(AuthorizedUser));
        }

        [AuthorizeAnyType(Type = AuthorizeType.Cookie | AuthorizeType.JWT)]
        [HttpPut("self")]
        public async Task<IActionResult> UpdateSelf([FromBody] UserUpdateRequest request)
        {
            if (request.FirstName is not null)
            {
                AuthorizedUser.FirstName = request.FirstName;
            }
            if (request.LastName is not null)
            {
                AuthorizedUser.LastName = request.LastName;
            }
            if (request.Email is not null)
            {
                AuthorizedUser.Email = request.Email;
            }
            await DB.SaveChangesAsync();
            return Ok(new UserInfoResponse(AuthorizedUser));
        }
    }
}
