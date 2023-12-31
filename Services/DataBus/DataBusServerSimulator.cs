using ShineSyncControl.Services.DataBus.Models;

namespace ShineSyncControl.Services.DataBus
{
    public class DataBusServerSimulator : IDataBus
    {
        private readonly Dictionary<string, List<Action<DataBusResponse>>> subscribers = new Dictionary<string, List<Action<DataBusResponse>>>();
        private readonly Queue<Request> requests = new Queue<Request>();

        private Task workFlow;

        public DataBusServerSimulator()
        {
            workFlow = Task.Run(WorkFlow, default);
        }

        public void Publish(string topic, string message)
        {
            requests.Enqueue(new Request(topic, message));
        }

        public void Subscribe(string topic, Action<DataBusResponse> action)
        {
            lock (subscribers)
            {
                if (!subscribers.ContainsKey(topic))
                {
                    subscribers[topic] = new List<Action<DataBusResponse>>();
                }
                subscribers[topic].Add(action);
            }
        }

        public void Unsubscribe(string topic, Action<DataBusResponse> action)
        {
            lock (subscribers[topic])
            {
                subscribers[topic].Remove(action);
                if (subscribers[topic].Count == 0)
                {
                    subscribers.Remove(topic);
                }
            }
        }

        private async void WorkFlow()
        {
            while (true)
            {
                if (requests.TryDequeue(out var item) && item is not null && subscribers.TryGetValue(item.Topic, out var hendeler))
                {
                    foreach (var action in hendeler)
                    {
                        action(new DataBusResponse(item.Topic, item.Message));
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
