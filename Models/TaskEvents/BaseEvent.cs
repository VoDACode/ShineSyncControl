using ShineSyncControl.Models.DB;
using ShineSyncControl.Services.TaskEventWorker;

namespace ShineSyncControl.Models.TaskEvents
{
    public abstract class BaseEvent
    {
        public abstract void Execute(TaskModel taskModel, TaskEventWorkerArgs args);
    }
}
