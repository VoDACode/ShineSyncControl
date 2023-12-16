using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses.WebSocket
{
    public class PropertyWebSocketResponse
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public PropertyType Type { get; set; }
        public string? TypeText => Enum.GetName(Type);

        public PropertyWebSocketResponse(DeviceProperty property)
        {
            Name = property.Name;
            Value = property.Value;
            Type = property.Type;
        }
    }
}
