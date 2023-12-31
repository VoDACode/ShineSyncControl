using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.TaskEvents;
using ShineSyncControl.Services.DataBus;
using System.Reflection;

namespace ShineSyncControl.Services.TaskEventWorker
{
    public class TaskEventWorker : ITaskEventWorker
    {
        private Dictionary<string, BaseEvent> baseEvents = new Dictionary<string, BaseEvent>();

        private List<string> eventsNames { get; } = new List<string>();
        public IReadOnlyList<string> Events => eventsNames;

        protected readonly IDistributedCache cache;
        protected readonly IDataBus dataBus;

        public TaskEventWorker(IDistributedCache cache, IDataBus dataBus)
        {
            this.cache = cache;
            this.dataBus = dataBus;
            LoadClasses();
        }

        public void Execute(TaskModel taskModel)
        {
            if(!baseEvents.TryGetValue(taskModel.EventName, out var baseEvent))
            {
                return;
            }
            var args = new TaskEventWorkerArgs(cache, dataBus);
            baseEvent.Execute(taskModel, args);
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
