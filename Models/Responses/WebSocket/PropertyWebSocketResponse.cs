using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses.WebSocket
{
    public class PropertyWebSocketResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public PropertyType Type { get; set; }
        public string? TypeText => Enum.GetName(Type);

        public PropertyWebSocketResponse(DeviceProperty property)
        {
            Id = property.Id;
            Name = property.Name;
            Value = property.Value;
            Type = property.Type;
        }
        public PropertyWebSocketResponse() { }
    }
}
