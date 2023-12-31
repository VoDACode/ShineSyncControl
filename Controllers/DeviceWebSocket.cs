using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Responses.WebSocket;
using ShineSyncControl.Services.DataBus;
using ShineSyncControl.Services.DataBus.Models;
using ShineSyncControl.Services.DeviceCommand;
using System.Security.Claims;
using System.Text.Json;
using VoDA.WebSockets;

namespace ShineSyncControl.Controllers
{
    [Route("api/device/ws")]
    public class DeviceWebSocket : BaseWebSocketController
    {
        protected readonly DbApp db;
        protected readonly IDeviceCommandService deviceCommandService;
        protected readonly IDistributedCache cache;
        protected readonly IDataBus dataBus;

        private DateTime lastPing = DateTime.UtcNow;

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

        public DeviceWebSocket(DbApp db, IDeviceCommandService deviceCommandService, IDistributedCache cache, IDataBus dataBus)
        {
            this.db = db;
            this.deviceCommandService = deviceCommandService;
            this.cache = cache;
            this.dataBus = dataBus;
        }

        [AuthorizeAnyType(Type = Enums.AuthorizeType.Device)]
        [WebSocketCyclic]
        [WebSocketPath("command")]
        public async void Command(string message)
        {
            if(string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message == "ping")
            {
                var timeNow = DateTime.UtcNow;
                await cache.SetStringAsync($"device_{Client.Id}_online",
                    timeNow.ToString(),
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTime.UtcNow.AddMinutes(1)
                    });
                var device = HttpContext.Items["Device"] as Device;
                if (device is not null)
                {
                    device.LastOnline = timeNow;
                    db.Devices.Update(device);
                    await db.SaveChangesAsync();
                }
                return;
            }

            deviceCommandService.HandleCommand(new DeviceCommandContext(message, HttpContext, Client));
            if ((DateTime.UtcNow - lastPing).TotalSeconds > 30)
            {
                lastPing = DateTime.UtcNow;
                await Client.SendAsync("ping");
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
            // DeviceWebSocketResponse response = new DeviceWebSocketResponse(device);
            try
            {
                dataBus.Subscribe($"device_{device.Id}", DeviceUpdateLiscener);
                while (!this.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
                dataBus.Unsubscribe($"device_{device.Id}", DeviceUpdateLiscener);
                //while (true)
                //{
                //    var lastOnlineString = await cache.GetStringAsync($"device_{id}_online");
                //    response.LastOnline = (lastOnlineString is not null ? DateTime.Parse(lastOnlineString) : device.LastOnline) ?? DateTime.MinValue;

                //    foreach (var property in response.Properties)
                //    {
                //        var value = await cache.GetStringAsync($"device_{property.Id}.value");
                //        if (value is not null)
                //        {
                //            property.Value = value;
                //        }
                //    }

                //    await Client.SendAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions()
                //    {
                //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                //    }));
                //    await Task.Delay(5_000);
                //}
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
