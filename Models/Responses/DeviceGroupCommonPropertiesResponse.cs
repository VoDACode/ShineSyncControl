using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class DeviceGroupCommonPropertiesResponse : BaseResponse
    {
        public DeviceGroupCommonPropertiesResponse(bool success, object? data = null) : base(success, data)
        {
        }

        public class View
        {
            public string PropertyName { get; set; }
            public string Value { get; set; }
            public string? PropertyUnit { get; set; } = null;
            public PropertyType Type { get; set; }
            public string TypeText { get; set; }

            public View(DeviceProperty property)
            {
                this.PropertyName = property.Name;
                this.Value = property.Value;
                this.PropertyUnit = property.PropertyUnit;
                this.Type = property.Type;
                this.TypeText = Enum.GetName(this.Type) ?? "none";
            }
        }
    }
}
