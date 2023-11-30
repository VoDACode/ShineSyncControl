using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShineSyncControl.Attributes.AuthorizeAnyType;
using ShineSyncControl.Enums;
using System.Linq;
using System.Security.Claims;

namespace ShineSyncControl.Attributes
{
    public class AuthorizeAnyTypeAttribute : Attribute, IAuthorizationFilter
    {
        public AuthorizeType Type { get; set; } = AuthorizeType.Any;
        public string? Roles { get; set; } = null;

        private int intType => (int)Type;
        private string[]? roles => Roles?.Split(',');

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext? http = context.HttpContext;

            if (http is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool isDevice = http.Request.Headers.ContainsKey("DeviceId") && http.Request.Headers.ContainsKey("Token");
            bool isCookie = http.Request.Cookies.Any(p => p.Key == ".VoDACode.Authorize");
            bool isJWT = http.Request.Headers.Any(p => p.Key == "Authorization");

            IAuthorizationFilter? handeled = isCookie && ValidateType(AuthorizeType.Cookie) ? new AuthorizeCookie() :
                                             isJWT && ValidateType(AuthorizeType.JWT) ? new AuthorizeJWT() :
                                             isDevice && ValidateType(AuthorizeType.Device) ? new AuthorizeDevice() :
                                             null;

            if(handeled is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            handeled.OnAuthorization(context);

            if (context.Result is null && Roles is not null && !isDevice)
            {
                var userRoles = context.HttpContext.User.FindFirst(ClaimTypes.Role);
                if (userRoles != null)
                {
                    var parsedRole = userRoles.Value.Split(',');

                    foreach (var role in parsedRole)
                    {
                        if (roles?.Contains(role) == true)
                        {
                            return;
                        }
                    }
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }

        bool ValidateType(AuthorizeType type)
        {
            if ((intType & (int)AuthorizeType.Any) == (int)AuthorizeType.Any)
                return true;
            return (intType & (int)type) == (int)type;
        }
    }
}
