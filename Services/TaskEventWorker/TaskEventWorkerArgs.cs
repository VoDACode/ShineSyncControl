using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Services.DataBus;

namespace ShineSyncControl.Services.TaskEventWorker
{
    public class TaskEventWorkerArgs
    {
        public IDistributedCache Cache { get; }
        public IDataBus DataBus { get; }

        public TaskEventWorkerArgs(IDistributedCache cache, IDataBus dataBus)
        {
            Cache = cache;
            this.DataBus = dataBus;
        }
    }
}
