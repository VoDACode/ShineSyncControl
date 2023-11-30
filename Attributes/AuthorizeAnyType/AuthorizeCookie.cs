using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShineSyncControl.Attributes.AuthorizeAnyType
{
    public class AuthorizeCookie : AuthorizeJWT
    {
        public override void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext http = context.HttpContext;

            string? jwt = http.Request.Cookies[".VoDACode.Authorize"];

            if (jwt is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Authorize(context, jwt);
        }
    }
}
