using System.Text.RegularExpressions;

namespace ShineSyncControl.Services.DeviceCommand.Commands
{
    public abstract class BaseCommand
    {
        public abstract Regex Pattern { get; }
        public abstract bool HandleCommand(DeviceCommandContext context);

        public bool IsMatch(string message)
        {
            return Pattern.IsMatch(message);
        }

        protected Dictionary<string, string> GetParameters(string message)
        {
            var parameters = new Dictionary<string, string>();
            var match = Pattern.Match(message);
            if (match.Success)
            {
                foreach (var groupName in Pattern.GetGroupNames())
                {
                    parameters.Add(groupName, match.Groups[groupName].Value);
                }
            }
            return parameters;
        }
    }
}
