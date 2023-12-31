using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Responses.WebSocket;
using ShineSyncControl.Services.TaskEventWorker;
using System.Text.Json;

namespace ShineSyncControl.Models.TaskEvents
{
    [EventName("set")]
    public class SetTaskEvent : BaseEvent
    {
        public override void Execute(TaskModel taskModel, TaskEventWorkerArgs args)
        {
            taskModel.DeviceProperty.Value = taskModel.Value;
            args.DataBus.Publish($"device_{taskModel.DeviceId}", JsonSerializer.Serialize(new DeviceWebSocketResponse(taskModel.Device), new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            args.Cache.SetString($"device_{taskModel.DevicePropertyId}.value", taskModel.Value);
        }
    }
}
