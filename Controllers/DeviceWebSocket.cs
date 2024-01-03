using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Services.DataBus;
using ShineSyncControl.Services.DataBus.Models;
using ShineSyncControl.Services.DeviceManager;
using System.Security.Claims;
using VoDA.WebSockets;

namespace ShineSyncControl.Controllers
{
    [Route("api/device/ws")]
    public class DeviceWebSocket : BaseWebSocketController
    {
        protected readonly DbApp db;
        protected readonly IDataBus dataBus;
        protected readonly IDeviceManager deviceManager;

        protected int AuthorizedUserId
        {
            get
            {
                var strId = HttpContext.User.Claims
                    .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

                if (strId is null)
                {
                    throw new InvalidOperationException("This property accessible only for authorized users.");
                }

                if (!int.TryParse(strId, out int id))
                {
                    throw new InvalidOperationException("This property accessible only for authorized users.");
                }

                return id;
            }
        }

        public DeviceWebSocket(DbApp db, IDataBus dataBus, IDeviceManager deviceManager)
        {
            this.db = db;
            this.dataBus = dataBus;
            this.deviceManager = deviceManager;
        }

        [AuthorizeAnyType(Type = Enums.AuthorizeType.Device)]
        [WebSocketPath("connect")]
        public async Task Connect()
        {
            var device = HttpContext.Items["Device"] as Device;
            if (device is null)
            {
                return;
            }
            device = db.Devices.Include(x => x.Properties).Single(p => p.Id == device.Id);

            DeviceContext context = new DeviceContext(device, Client, HttpContext);
            if (deviceManager.Register(context))
            {
                device.LastOnline = DateTime.UtcNow;
                await db.SaveChangesAsync();
                await context.Send("Hello!");
                dataBus.Subscribe($"device:{device.Id}", context.SendDataBusData);
                deviceManager.WaitForDisconnect(device.Id);
                deviceManager.Unregister(context);
                dataBus.Unsubscribe($"device:{device.Id}", context.SendDataBusData);
                await context.Send("Bye!");
                device.LastOnline = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
        }

        [AuthorizeAnyType(Type = Enums.AuthorizeType.User)]
        [WebSocketPath("{id}/update")]
        public async Task DeviceUpdate(string id)
        {
            var device = await db.Devices.Include(x => x.Properties).SingleOrDefaultAsync(p => p.Id == id);
            if (device == null || device.UserId != AuthorizedUserId)
            {
                return;
            }
            try
            {
                dataBus.Subscribe($"device:{device.Id}", DeviceUpdateLiscener);
                while (!this.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
                dataBus.Unsubscribe($"device:{device.Id}", DeviceUpdateLiscener);
            }
            catch
            {

            }

            void DeviceUpdateLiscener(DataBusResponse response)
            {
                Client.SendAsync(response.Message).Wait();
            }
        }
    }
}
