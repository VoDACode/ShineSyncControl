using Microsoft.AspNetCore.Mvc;
using ShineSyncControl.Attributes;
using ShineSyncControl.Services.DeviceCommand;
using VoDA.WebSockets;

namespace ShineSyncControl.Controllers
{
    [Route("api/device/ws")]
    public class DeviceWebSocket : BaseWebSocketController
    {
        protected readonly DbApp db;
        protected readonly IDeviceCommandService deviceCommandService;
        public DeviceWebSocket(DbApp db, IDeviceCommandService deviceCommandService)
        {
            this.db = db;
            this.deviceCommandService = deviceCommandService;
        }

        //[AuthorizeAnyType(Type = Enums.AuthorizeType.Device)]
        [WebSocketCyclic]
        [WebSocketPath("command")]
        public async void Command(string message)
        {
            deviceCommandService.HandleCommand(new DeviceCommandContext(message, HttpContext, Client));
        }
    }
}
