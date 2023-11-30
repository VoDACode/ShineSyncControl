using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.TaskEvents
{
    [EventName("set")]
    public class SetTaskEvent : BaseEvent
    {
        public override void Execute(TaskModel taskModel)
        {
            taskModel.DeviceProperty.Value = taskModel.Value;
        }
    }
}
