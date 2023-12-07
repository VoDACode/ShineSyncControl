using Newtonsoft.Json;

namespace ShineSyncControl.Services.DeviceCommand.Models.Requests
{
    public class CommandRequestPattern
    {
        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
