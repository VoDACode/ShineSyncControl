using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Services.TaskEventWorker
{
    public interface ITaskEventWorker
    {
        public IReadOnlyList<string> Events { get; }
        void Execute(TaskModel taskModel);
    }
}
