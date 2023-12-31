using ShineSyncControl.Services.DataBus.Models;

namespace ShineSyncControl.Services.DataBus
{
    public interface IDataBus
    {
        public void Publish(string topic, string message);
        public void Subscribe(string topic, Action<DataBusResponse> action);
        public void Unsubscribe(string topic, Action<DataBusResponse> action);
    }
}
