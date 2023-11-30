using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Attributes.AuthorizeAnyType
{
    public class AuthorizeDevice : IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext? http = context.HttpContext;

            var dbContext = context.HttpContext
            .RequestServices
            .GetService(typeof(DbApp)) as DbApp;

            if (dbContext == null)
            {
                throw new ArgumentNullException("DbApp is null");
            }

            string? deviceId = http.Request.Headers["DeviceId"];
            string? token = http.Request.Headers["Token"];

            if (deviceId == null || token == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Device? device = dbContext.Devices.SingleOrDefault(d => d.Id == deviceId && d.Token == token);
            if (device is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items.Add("Device", device);
        }
    }
}
