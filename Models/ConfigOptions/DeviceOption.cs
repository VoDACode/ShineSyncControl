namespace ShineSyncControl.Models.ConfigOptions
{
    public class DeviceOption
    {
        public int ActivationTimeOut { get; set; }
        public ICollection<DeviceTypeOption> Types { get; set; }
    }
}
