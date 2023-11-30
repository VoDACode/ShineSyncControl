using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.TaskEvents;
using System.Reflection;

namespace ShineSyncControl.Services.TaskEventWorker
{
    public class TaskEventWorker : ITaskEventWorker
    {
        private Dictionary<string, BaseEvent> baseEvents = new Dictionary<string, BaseEvent>();

        private List<string> eventsNames { get; } = new List<string>();
        public IReadOnlyList<string> Events => eventsNames;

        public TaskEventWorker()
        {
            LoadClasses();
        }

        public void Execute(TaskModel taskModel)
        {
            if(!baseEvents.TryGetValue(taskModel.EventName, out var baseEvent))
            {
                return;
            }
            baseEvent.Execute(taskModel);
        }

        private void LoadClasses()
        {
            baseEvents.Clear();

            var targetClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(p => p.Namespace == "ShineSyncControl.Models.TaskEvents" && p.BaseType == typeof(BaseEvent) && p.GetCustomAttribute(typeof(EventNameAttribute)) is not null);

            foreach (var targetClass in targetClasses)
            {
                var eventNameAttrebute = targetClass.GetCustomAttribute<EventNameAttribute>();
                if (eventNameAttrebute is null)
                {
                    continue;
                }

                var obj = Activator.CreateInstance(targetClass) as BaseEvent;
                if(obj is null)
                {
                    continue;
                }

                baseEvents.Add(eventNameAttrebute.EventName, obj);
                eventsNames.Add(eventNameAttrebute.EventName);
            }
        }
    }
}
