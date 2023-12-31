namespace ShineSyncControl.Services.DataBus.Models
{
    public struct DataBusResponse
    {
        public string Topic { get; }
        public string Message { get; }

        public DataBusResponse(string topic, string message)
        {
            Topic = topic;
            Message = message;
        }
    }
}
