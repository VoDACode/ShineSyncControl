using ShineSyncControl.Models.DB;
using ShineSyncControl.Services.DataBus;
using ShineSyncControl.Services.DataBus.Models;
using ShineSyncControl.Services.DeviceCommand;
using System.Net.WebSockets;
using VoDA.WebSockets;

namespace ShineSyncControl.Services.DeviceManager
{
    public class DeviceContext
    {
        public string Id => Device.Id;
        public Device Device { get; }
        public WebSocketClient WebSocketClient { get; }
        public HttpContext HttpContext { get; }
        public IDataBus DataBus { get; }
        public IDeviceCommandService DeviceCommandService { get; }

        private Task readTask { get; }

        public DeviceContext(Device device, WebSocketClient webSocketClient, HttpContext httpContext)
        {
            Device = device;
            WebSocketClient = webSocketClient;
            HttpContext = httpContext;

            DataBus = httpContext.RequestServices.GetService<IDataBus>() ?? throw new TypeLoadException(nameof(IDataBus));
            DeviceCommandService = httpContext.RequestServices.GetService<IDeviceCommandService>() ?? throw new TypeLoadException(nameof(IDeviceCommandService));

            readTask = Task.Run(async () => await Read());
        }

        private async Task Read()
        {
            while (WebSocketClient.Socket.State != WebSocketState.Closed && !HttpContext.RequestAborted.IsCancellationRequested)
            {
                string message = await WebSocketClient.ReceiveAsync();
                var context = new DeviceCommandContext(message, HttpContext, WebSocketClient);
                DeviceCommandService.HandleCommand(context);
            }
        }

        public void SendDataBusData(DataBusResponse response)
        {
            Send(response.Message).Wait();
        }

        public async Task Send(string message)
        {
            if (WebSocketClient.Socket.State == WebSocketState.Closed)
            {
                return;
            }
            await WebSocketClient.SendAsync(message);
        }
    }
}
