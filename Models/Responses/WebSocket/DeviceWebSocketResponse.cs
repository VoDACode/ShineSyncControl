using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses.WebSocket
{
    public class DeviceWebSocketResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime LastOnline { get; set; }
        public PropertyWebSocketResponse[] Properties { get; set; }

        public DeviceWebSocketResponse(Device device)
        {
            Id = device.Id;
            Name = device.Name ?? "";
            Type = device.Type ?? "unknown";
            LastOnline = device.LastOnline ?? DateTime.MinValue;
            Properties = device.Properties.Select(p => new PropertyWebSocketResponse(p)).ToArray();
        }

        public DeviceWebSocketResponse() { }
    }
}
