using ShineSyncControl.Services.DeviceCommand.Models.Responses;
using System.Text.Json;
using VoDA.WebSockets;

namespace ShineSyncControl.Services.DeviceCommand
{
    public class DeviceCommandContext
    {
        public string Message { get; }
        public HttpContext HttpContext { get; }
        public WebSocketClient WebSocketClient { get; }

        public DeviceCommandContext(string message, HttpContext httpContext, WebSocketClient webSocketClient)
        {
            Message = message;
            HttpContext = httpContext;
            WebSocketClient = webSocketClient;
        }

        public void Response(object response)
        {
            if (WebSocketClient.Socket.CloseStatus != null)
                return;
            WebSocketClient.SendAsync(JsonSerializer.Serialize(response));
        }
    }
}
