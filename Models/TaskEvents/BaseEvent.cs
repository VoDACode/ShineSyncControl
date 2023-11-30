using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.TaskEvents
{
    public abstract class BaseEvent
    {
        public abstract void Execute(TaskModel taskModel);
    }
}
