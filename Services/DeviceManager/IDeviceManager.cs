using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Services.DeviceManager
{
    public interface IDeviceManager
    {
        bool Register(DeviceContext context);
        bool Unregister(DeviceContext context);
        bool Unregister(string deviceId);
        void WaitForDisconnect(string deviceId);

        Task SendCommand(string deviceId, string command);

        DeviceContext? GetDeviceContext(string deviceId);
    }
}
