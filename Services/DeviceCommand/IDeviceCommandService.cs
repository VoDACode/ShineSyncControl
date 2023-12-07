namespace ShineSyncControl.Services.DeviceCommand
{
    public interface IDeviceCommandService
    {
        public bool HandleCommand(DeviceCommandContext context);
    }
}
