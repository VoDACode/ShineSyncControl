using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Services.DeviceCommand.Models.Responses;
using System.Text.RegularExpressions;

namespace ShineSyncControl.Services.DeviceCommand.Commands
{
    public class ScheduledTasksCommand : BaseCommand
    {
        public override Regex Pattern => new Regex("^schedule (?<arg1>list|help)$");

        public override bool HandleCommand(DeviceCommandContext context)
        {
            var commandParams = GetParameters(context.Message);
            var arg1 = commandParams["arg1"];
            var device = context.HttpContext.Items["Device"] as Device;

            DbApp? db = context.HttpContext.RequestServices.GetService<DbApp>();

            if(db == null)
            {
                throw new Exception("DbApp not found in services");
            }

            if (arg1 == "list")
            {
                var tasks = db.ScheduledTasks
                    .Include(x => x.Task)
                    .Where(x => x.Task.DeviceId == device.Id)
                    .ToList();

                context.Response(new CommandScheduledTasksResponse(tasks));
                return true;
            }

            return true;
        }
    }
}
