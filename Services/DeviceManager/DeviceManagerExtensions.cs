using ShineSyncControl.Services.DeviceCommand;

namespace ShineSyncControl.Services.DeviceManager
{
    public static class DeviceManagerExtensions
    {
        public static IServiceCollection AddDeviceManager(this IServiceCollection services)
        {
            if(!services.Any(p => p.ServiceType == typeof(IDeviceCommandService)))
            {
                services.AddSingleton<IDeviceCommandService, DeviceCommandService>();
            }
            services.AddSingleton<IDeviceManager, DeviceManager>();
            return services;
        }
    }
}
