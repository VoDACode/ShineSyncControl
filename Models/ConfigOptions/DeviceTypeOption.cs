namespace ShineSyncControl.Models.ConfigOptions
{
    public class DeviceTypeOption
    {
        public string Type { get; set; }
        public ICollection<DevicePropertyOption> Properties { get; set; }
    }
}
