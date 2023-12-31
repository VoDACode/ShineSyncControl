using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Services.DeviceCommand.Models.Responses
{
    public class CommandTaskResponse : BaseCommandResponse
    {
        public IEnumerable<View> Tasks { get; set; }
        public CommandTaskResponse(IEnumerable<TaskModel> tasks) : base(true, null)
        {
            Tasks = tasks.Select(t => new View(t));
        }

        public class View
        {
            public int Id { get; set; }
            public string PropertyName { get; set; }
            public string Value { get; set; }
            public string Event { get; set; }
            public long? Interval { get; set; }
            public bool Enabled { get; set; }

            public View(TaskModel task)
            {
                Id = task.Id;
                PropertyName = task.DevicePropertyId;
                Value = task.Value;
                Event = task.EventName;
                Interval = task.Interval;
                Enabled = task.Enabled;
            }
        }
    }
}
