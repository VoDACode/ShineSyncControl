namespace ShineSyncControl.Services.DataBus
{
    public static class DataBusExtension
    {
        public static IServiceCollection AddDataBus(this IServiceCollection services)
        {
            services.AddSingleton<IDataBus, DataBusServerSimulator>();
            return services;
        }
    }
}
