using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Services.DeviceCommand.Models.Responses;
using System.Text.RegularExpressions;

namespace ShineSyncControl.Services.DeviceCommand.Commands
{
    public class ValueCommand : BaseCommand
    {
        public override Regex Pattern => new Regex("^set-value (?<property>[A-Za-z0-9]+) (?<value>.*?)$");

        public override bool HandleCommand(DeviceCommandContext context)
        {
            DbApp db = context.HttpContext.RequestServices.GetService<DbApp>() ?? throw new ArgumentNullException(nameof(context));
            IDistributedCache cache = context.HttpContext.RequestServices.GetService<IDistributedCache>() ?? throw new ArgumentNullException(nameof(context));

            var device = context.HttpContext.Items["Device"] as Device;

            var patams = GetParameters(context.Message);
            var property = patams["property"];
            var value = patams["value"];
            if (device is null)
            {
                context.Response(new BaseCommandResponse.ErrorResponse("Device not found"));
                return true;
            }

            var deviceProperty = db.DeviceProperties.SingleOrDefault(p => p.DeviceId == device.Id && p.Name == property);
            if (deviceProperty is null)
            {
                context.Response(new BaseCommandResponse.ErrorResponse("Property not found"));
                return true;
            }
            if (!deviceProperty.TrySetValue(value))
            {
                context.Response(new BaseCommandResponse.ErrorResponse($"'{value}' is not {Enum.GetName(deviceProperty.Type)}"));
                return true;
            }
            deviceProperty.PropertyLastSync = DateTime.UtcNow;
            device.LastSync = DateTime.UtcNow;
            device.LastOnline = DateTime.UtcNow;

            cache.SetString($"device_{deviceProperty.Id}.value", value);

            db.SaveChanges();
            context.Response(new BaseCommandResponse.SuccessResponse());
            return true;
        }
    }
}
