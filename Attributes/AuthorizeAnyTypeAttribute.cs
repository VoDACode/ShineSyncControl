using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShineSyncControl.Attributes.AuthorizeAnyType;
using ShineSyncControl.Enums;
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

            if(http is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string? authorizeType = http.Request.Headers["Authorization-Type"];

            IAuthorizationFilter? handeled = null;

            if ((this.intType & (int)AuthorizeType.Any) == 0)
            {
                if (authorizeType is null && (this.intType & (int)AuthorizeType.Cookie) == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                if (authorizeType == "Device" && (this.intType & (int)AuthorizeType.Device) == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                if (authorizeType == "JWT" && (this.intType & (int)AuthorizeType.JWT) == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            switch (authorizeType)
            {
                case "Device":
                    handeled = new AuthorizeDevice();
                    break;
                case "JWT":
                    handeled = new AuthorizeJWT();
                    break;
                default:
                    handeled = new AuthorizeCookie();
                    break;
            }

            handeled.OnAuthorization(context);

            if (context.Result is null && Roles is not null && authorizeType != "Device")
            {
                var userRoles = context.HttpContext.User.FindFirst(ClaimTypes.Role);
                if(userRoles != null)
                {
                    var parsedRole = userRoles.Value.Split(',');

                    foreach (var role in parsedRole)
                    {
                        if(roles?.Contains(role) == true)
                        {
                            return;
                        }
                    }
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
