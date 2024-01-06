using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShineSyncControl.Models.DB;
using System.Security.Claims;

namespace ShineSyncControl.Controllers
{
    public class BaseController : ControllerBase
    {
        private int? authorizedUserId = null;
        private User? authorizedUser = null;
        protected User AuthorizedUser
        {
            get
            {
                if(authorizedUser is not null)
                {
                    return authorizedUser;
                }

                int id = AuthorizedUserId;

                User? user = DB.Users.SingleOrDefault(p => p.Id == id);

                if(user is null)
                {
                    HttpContext.Response.Cookies.Delete(".VoDACode.Authorize");
                    Response.StatusCode = 401;
                    Response.WriteAsync("Unauthorized").Wait();
                    throw new InvalidOperationException("This property accessible only for authorized users.");
                }
                authorizedUser = user;

                return user;
            }
        }

        protected int AuthorizedUserId
        {
            get
            {
                if (authorizedUserId is not null)
                {
                    return authorizedUserId.Value;
                }

                var strId = HttpContext.User.Claims
                    .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

                if(strId is null)
                {
                    throw new InvalidOperationException("This property accessible only for authorized users.");
                }

                if (!int.TryParse(strId, out int id))
                {
                    throw new InvalidOperationException("This property accessible only for authorized users.");
                }

                authorizedUserId = id;

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
