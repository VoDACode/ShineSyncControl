using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class DevicePropertyResponse : BaseResponse
    {
        public DevicePropertyResponse(DeviceProperty property) : base(true, null)
        {
            Data = new View(property);
        }

        public DevicePropertyResponse(IEnumerable<DeviceProperty> property) : base(true, null)
        {
            Data = property.Select(p => new View(p));
        }

        public class View
        {
            public string DeviceId { get; set; }
            public string PropertyName { get; set; }
            public string Value { get; set; }
            public bool IsReadOnly { get; set; }
            public string? PropertyUnit { get; set; } = null;
            public DateTime PropertyLastSync { get; set; }
            public PropertyType Type { get; set; }
            public string TypeText { get; set; }


            public View(DeviceProperty property)
            {
                this.DeviceId = property.DeviceId;
                this.PropertyName = property.PropertyName;
                this.Value = property.Value;
                this.IsReadOnly = property.IsReadOnly;
                this.PropertyUnit = property.PropertyUnit;
                this.PropertyLastSync = property.PropertyLastSync;
                this.Type = property.Type;
                this.TypeText = Enum.GetName(this.Type) ?? "none";
            }
        }
    }
}
