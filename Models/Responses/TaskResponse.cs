using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class TaskResponse : BaseResponse
    {
        public TaskResponse(TaskModel task) : base(true, new View(task))
        {
        }

        public TaskResponse(IEnumerable<TaskModel> tasks) : base(true, tasks.Select(p => new View(p)))
        {
        }

        public class View
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DeviceResponse.View Device { get; set; }
            public DevicePropertyResponse.View Property { get; set; }
            public string EventName { get; set; }
            public string Value { get; set; }
            public PropertyType Type { get; set; }
            public string? Description { get; set; }
            public long? Interval { get; set; }
            public bool Enabled { get; set; }

            public View(TaskModel task)
            {
                Id = task.Id;
                Name = task.Name;
                Device = new DeviceResponse.View(task.Device);
                Property = new DevicePropertyResponse.View(task.DeviceProperty);
                EventName = task.EventName;
                Value = task.Value;
                Type = task.Type;
                Description = task.Description;
                Interval = task.Interval;
                Enabled = task.Enabled;
            }
        }
    }
}
