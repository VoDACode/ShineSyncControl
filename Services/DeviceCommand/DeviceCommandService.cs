using ShineSyncControl.Services.DeviceCommand.Commands;
using System.Reflection;

namespace ShineSyncControl.Services.DeviceCommand
{
    public class DeviceCommandService : IDeviceCommandService
    {
        private readonly List<BaseCommand> commands = new List<BaseCommand>();

        public DeviceCommandService()
        {
            LoadCommands();
        }

        public bool HandleCommand(DeviceCommandContext context)
        {
            var command = commands.FirstOrDefault(c => c.IsMatch(context.Message));
            if (command != null)
            {
                return command.HandleCommand(context);
            }
            return false;
        }

        private void LoadCommands()
        {
            var targetClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(p => p.Namespace == "ShineSyncControl.Services.DeviceCommand.Commands" && !p.IsAbstract && p.IsAssignableTo(typeof(BaseCommand)));
            foreach (var targetClass in targetClasses)
            {
                var command = Activator.CreateInstance(targetClass) as BaseCommand;
                if (command != null)
                {
                    commands.Add(command);
                }
            }
        }
    }
}
