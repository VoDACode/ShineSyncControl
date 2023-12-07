namespace ShineSyncControl.Services.DeviceCommand
{
    public static class DeviceCommandServiceExtensions
    {
        public static IServiceCollection AddDeviceCommand(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceCommandService, DeviceCommandService>();
            return services;
        }
    }
}
