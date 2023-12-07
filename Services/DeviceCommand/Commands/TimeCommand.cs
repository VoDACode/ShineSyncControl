using ShineSyncControl.Services.DeviceCommand.Models.Responses;
using System.Text.RegularExpressions;

namespace ShineSyncControl.Services.DeviceCommand.Commands
{
    public class TimeCommand : BaseCommand
    {
        public override Regex Pattern => new Regex("^time.*?$");

        public override bool HandleCommand(DeviceCommandContext context)
        {
            context.Response(new CommandTimeResponse());
            return true;
        }
    }
}
