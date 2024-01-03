using Microsoft.Extensions.Caching.Distributed;

namespace ShineSyncControl.Services.DeviceManager
{
    public class DeviceManager : IDeviceManager
    {
        private readonly HashSet<DeviceContext> devices = new HashSet<DeviceContext>();
        private readonly IDistributedCache cache;

        public DeviceManager(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public DeviceContext? GetDeviceContext(string deviceId) => devices.SingleOrDefault(p => p.Id == deviceId);

        public bool Register(DeviceContext context)
        {
            if (devices.Contains(context))
            {
                return false;
            }
            devices.Add(context);
            return true;
        }

        public async Task SendCommand(string deviceId, string command)
        {
            var device = GetDeviceContext(deviceId);
            if (device == null)
            {
                throw new Exception($"Device {deviceId} not found");
            }
            if (device.HttpContext.RequestAborted.IsCancellationRequested)
            {
                throw new Exception($"Device {deviceId} is disconnected");
            }
            await device.WebSocketClient.SendAsync(command);
        }

        public bool Unregister(DeviceContext context) => devices.Remove(context);

        public bool Unregister(string deviceId)
        {
            var device = GetDeviceContext(deviceId);
            if (device == null)
            {
                return false;
            }
            return Unregister(device);
        }

        public async void WaitForDisconnect(string deviceId)
        {
            var device = GetDeviceContext(deviceId);
            if (device == null)
            {
                return;
            }
            while (!device.HttpContext.RequestAborted.IsCancellationRequested)
            {
                await this.cache.SetStringAsync($"device:{deviceId}:online", "true", new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1_500)
                });
                await Task.Delay(1000);
            }
        }
    }
}
