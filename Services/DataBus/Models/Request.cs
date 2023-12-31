namespace ShineSyncControl.Services.DataBus.Models
{
    internal class Request
    {
        public string Topic { get; set; }
        public string Message { get; set; }

        public Request(string topic, string message)
        {
            Topic = topic;
            Message = message;
        }
    }
}
