using Microsoft.AspNetCore.Mvc;
using ShineSyncControl.Models.DB;
using System.Security.Claims;

namespace ShineSyncControl.Controllers
{
    public class BaseController : ControllerBase
    {
        protected User? AuthorizedUser
        {
            get
            {
                int? id = AuthorizedUserId;

                if(id == null)
                    return null;

                User? user = DB.Users.SingleOrDefault(p => p.Id == id);

                return user;
            }
        }

        protected int? AuthorizedUserId
        {
            get
            {
                var strId = HttpContext.User.Claims
                    .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(strId, out int id))
                {
                    return null;
                }

                return id;
            }
        }


        protected readonly DbApp DB;
        public BaseController(DbApp db)
        {
            this.DB = db;
        }
    }
}
