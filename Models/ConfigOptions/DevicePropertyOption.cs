using ShineSyncControl.Enums;

namespace ShineSyncControl.Models.ConfigOptions
{
    public class DevicePropertyOption
    {
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public int TypeCode => (int)Enum.Parse(typeof(PropertyType), Type);
        public string? Units { get; set; }
        public bool IsReadOnly { get; set; } = false;
    }
}
