using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Services.DeviceCommand.Models.Responses
{
    public class CommandScheduledTasksResponse : BaseCommandResponse
    {
        public IEnumerable<View> Tasks { get; set; }
        public CommandScheduledTasksResponse(IEnumerable<ScheduledTask> tasks) : base(true, null)
        {
            Tasks = tasks.Select(t => new View(t));
        }

        public class View
        {
            public int Id { get; set; }
            public string PropertyName { get; set; }
            public string Value { get; set; }
            public string Event { get; set; }
            public long Interval { get; set; }
            public bool Enabled { get; set; }

            public View(ScheduledTask scheduledTask)
            {
                Id = scheduledTask.Id;
                PropertyName = scheduledTask.Task.DevicePropertyName;
                Value = scheduledTask.Task.Value;
                Event = scheduledTask.Task.EventName;
                Interval = scheduledTask.Interval;
                Enabled = scheduledTask.Enabled;
            }
        }
    }
}
