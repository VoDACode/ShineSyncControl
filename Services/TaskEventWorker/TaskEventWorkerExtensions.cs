namespace ShineSyncControl.Services.TaskEventWorker
{
    public static class TaskEventWorkerExtensions
    {
        public static void AddTaskEventWorker(this IServiceCollection services)
        {
            services.AddSingleton<ITaskEventWorker, TaskEventWorker>();
        }
    }
}
