using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [AuthorizeAnyType(Type = AuthorizeType.User)]
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseController
    {
        public UsersController(DbApp db) : base(db)
        {
        }

        [HttpGet("self")]
        public IActionResult GetSelf()
        {
            return Ok(new UserInfoResponse(AuthorizedUser));
        }

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

        [AuthorizeAnyType(Roles = $"{UserRoles.Admin}")]
        [HttpPost("{id}/block")]
        public async Task<IActionResult> BlockUser([FromRoute] int id)
        {
            var user = await DB.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user is null)
            {
                return NotFound(new BaseResponse.ErrorResponse());
            }
            user.IsBlocked = !user.IsBlocked;
            await DB.SaveChangesAsync();
            return Ok(new UserInfoResponse(user));
        }

        [AuthorizeAnyType(Roles = $"{UserRoles.Admin}")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole([FromRoute] int id, [FromBody] UserRoleRequest request)
        {
            var user = await DB.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user is null)
            {
                return NotFound(new BaseResponse.ErrorResponse("User not found."));
            }

            if (user.Id == AuthorizedUser.Id)
            {
                return BadRequest(new BaseResponse.ErrorResponse("You can't change your own role."));
            }

            if (user.Role == UserRoles.Admin && request.Role != UserRoles.Admin)
            {
                if (await DB.Users.CountAsync(p => p.Role == UserRoles.Admin) == 1)
                {
                    return BadRequest(new BaseResponse.ErrorResponse("You can't remove the last admin."));
                }
            }

            var role = await DB.Roles.SingleOrDefaultAsync(p => p.Role == request.Role);
            if (role is null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Role not found."));
            }

            user.Role = role.Role;
            user.RoleEntity = role;

            await DB.SaveChangesAsync();
            return Ok(new UserInfoResponse(user));
        }

        [AuthorizeAnyType(Roles = $"{UserRoles.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await DB.Users.SingleOrDefaultAsync(p => p.Id == id);
            if (user is null)
            {
                return NotFound(new BaseResponse.ErrorResponse());
            }
            if (user.Id == AuthorizedUser.Id)
            {
                return BadRequest(new BaseResponse.ErrorResponse("You can't delete your own account."));
            }

            if(user.Role == UserRoles.Admin && await DB.Users.CountAsync(p => p.Role == UserRoles.Admin) == 1)
            {
                return BadRequest(new BaseResponse.ErrorResponse("You can't remove the last admin."));
            }

            DB.Users.Remove(user);
            await DB.SaveChangesAsync();
            return Ok(new UserInfoResponse(user));
        }
    }
}
