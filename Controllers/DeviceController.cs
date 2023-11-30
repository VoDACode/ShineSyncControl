using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.TaskEventWorker;
using System.Security.Cryptography;
using Task = System.Threading.Tasks.Task;

namespace ShineSyncControl.Controllers
{
    [Route("api/device")]
    [ApiController]
    public class DeviceController : BaseController
    {
        protected readonly IDistributedCache cache;
        protected readonly ITaskEventWorker taskEventWorker;

        public DeviceController(DbApp db, IDistributedCache cache, ITaskEventWorker taskEventWorker) : base(db)
        {
            this.cache = cache;
            this.taskEventWorker = taskEventWorker;
        }

        [Authorize(Roles = $"{UserRoles.Registrar},{UserRoles.Admin}")]
        [HttpPost("register")]
        public async Task<IActionResult> RegiaterDevice([FromBody] RegisterDeviceRequest request)
        {
            var token = new byte[512];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(token);
            }

            var device = (await DB.AddAsync(new Device()
            {
                Id = $"{Guid.NewGuid()}-{DateTime.UtcNow.Ticks}",
                Type = request.Type,
                Token = Convert.ToBase64String(token).Replace("\\", "").Replace("/", "").Replace("=", "")
            })).Entity;

            await DB.SaveChangesAsync();

            var properties = request.Properties.Select(p => new DeviceProperty()
            {
                DeviceId = device.Id,
                PropertyName = p.PropertyName,
                IsReadOnly = p.IsReadOnly,
                Type = p.Type
            }).ToList();

            await DB.AddRangeAsync(properties);

            await DB.SaveChangesAsync();

            return Ok(new DeviceRegisteredResponse(device));
        }

        /*  Activation flow:
         *  
         *  (Optional) F1 Client setup WiFi connection on the device (use Blututh or something else)
         *  F1 Client sends a request to the server with the device id. Waiting for the device to connect to the server
         *  F2 Device connects to the server and sends the device id. Notificate the server that the device is connected and waiting for the activation
         *  F1 After the device is connected, make new device token (512 bits) and send it to the device (F2) and switch active flag to true
         *  F1 Send the OK response to the client
         *  
         */

        [AuthorizeAnyType]
        [HttpPut("activate")]
        public async Task<IActionResult> ActivateDevice([FromQuery] ActivateDeviceRequest request)
        {
            var device = await DB.Devices.FirstOrDefaultAsync(d => d.Id == request.DeviceId);
            if (device == null)
            {
                return NotFound(new BaseResponse.ErrorResponse());
            }
            if (device.IsActive)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Device is already active"));
            }

            await cache.SetStringAsync($"device:{request.DeviceId}:user:activate", "true");

            DateTime connectionStart = DateTime.Now;

            string? data = null;

            while ((data = await cache.GetStringAsync($"device:{request.DeviceId}:device:activate")) is null)
            {
                if (DateTime.Now > connectionStart + TimeSpan.FromHours(60))
                {
                    await cache.RemoveAsync($"device:{request.DeviceId}:user:activate");
                    return BadRequest(new BaseResponse.ErrorResponse("Timeout"));
                }
                await Task.Delay(1000);
            }

            device = await DB.Devices.SingleAsync(d => d.Id == request.DeviceId);
            device.OwnerId = AuthorizedUserId;
            await DB.SaveChangesAsync();

            await cache.RemoveAsync($"device:{request.DeviceId}:user:activate");
            await cache.RemoveAsync($"device:{request.DeviceId}:device:activate");

            return Ok(new BaseResponse.SuccessResponse());
        }

        [AuthorizeAnyType(Type = AuthorizeType.Device)]
        [HttpGet("waiting-activation")]
        public async Task<IActionResult> GetWaitingActivationDevices([FromHeader(Name = "DeviceId")] string deviceId, [FromHeader(Name = "Token")] string token)
        {
            Device? device = await DB.Devices.FirstOrDefaultAsync(d => d.Id == deviceId && d.Token == token);

            if (device == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device is not registered"));
            }

            if (device.IsActive)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Device is already active"));
            }

            await cache.SetStringAsync($"device:{deviceId}:device:activate", "true");

            DateTime connectionStart = DateTime.Now;
            string? data = null;

            while ((data = await cache.GetStringAsync($"device:{deviceId}:user:activate")) is null)
            {
                if (DateTime.Now > connectionStart + TimeSpan.FromHours(60))
                {
                    await cache.RemoveAsync($"device:{deviceId}:device:activate");
                    return BadRequest(new BaseResponse.ErrorResponse("Timeout"));
                }
                await Task.Delay(1000);
            }
            device = await DB.Devices.SingleAsync(d => d.Id == deviceId);

            var newToken = new byte[512];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newToken);
            }

            device.Token = Convert.ToBase64String(newToken).Replace("\\", "").Replace("/", "").Replace("=", "");
            device.ActivatedAt = DateTime.UtcNow;
            device.IsActive = true;
            await DB.SaveChangesAsync();

            return Ok(device.Token);
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any)]
        [HttpPut("{deviceId}/property/{propertyName}")]
        public async Task<IActionResult> UpdateProperty([FromRoute] string deviceId, [FromRoute] string propertyName, [FromBody] UpdatePropertyRequest request)
        {
            Device? device = HttpContext.Items["Device"] as Device;
            if (device is not null && device.Id != deviceId)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Device is not registered"));
            }

            DeviceProperty? property = await DB.DeviceProperties.Include(p => p.Device).FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.PropertyName == propertyName);
            if (property is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{propertyName}' not found"));
            }

            if(property.IsReadOnly && device is null)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Property '{propertyName}' is read only"));
            }

            if (!property.TrySetValue(request.Value))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid data type. Expected '{Enum.GetName(property.Type)}'"));
            }

            property.Device.LastSync = DateTime.UtcNow;
            property.PropertyLastSync = DateTime.UtcNow;

            await DB.SaveChangesAsync();

            var actionTasks = await DB.ActionTask
            .Include(a => a.WhenTrueTask).ThenInclude(p => p.DeviceProperty)
            .Include(a => a.WhenFalseTask).ThenInclude(p => p.DeviceProperty)
            .Include(a => a.Action).ThenInclude(p => p.Expression).ThenInclude(e => e.SubExpression).ThenInclude(p => p.DeviceProperty)
            .Where(a => (a.Action.Expression.DeviceId == deviceId && a.Action.Expression.DevicePropertyId == property.Id) ||
                        (a.Action.Expression.SubExpression != null && a.Action.Expression.SubExpression.DeviceId == deviceId && a.Action.Expression.SubExpression.DevicePropertyId == property.Id)
                      )
                .ToListAsync();

            foreach (var actionTask in actionTasks)
            {
                if (actionTask.Action.Expression.Execute())
                {
                    taskEventWorker.Execute(actionTask.WhenTrueTask);
                }
                else
                {
                    taskEventWorker.Execute(actionTask.WhenFalseTask);
                }
                await DB.SaveChangesAsync();
            }

            return Ok(new DevicePropertyResponse(property));
        }

        [AuthorizeAnyType]
        [HttpGet("{deviceId}/property")]
        public async Task<IActionResult> GetProperties([FromRoute] string deviceId, [FromHeader(Name = "Token")] string? token)
        {
            Device? device = await DB.Devices
                .Include(d => d.Properties)
                .Where(d => d.Id == deviceId)
                .SingleOrDefaultAsync();
            if (device is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Device '{deviceId}' not found"));
            }
            if (token is null && device.OwnerId != AuthorizedUserId)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Access is denied"));
            }
            else if (token is not null && device.Token != token)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Access is denied"));
            }

            return Ok(new DevicePropertyResponse(device.Properties));
        }

        [AuthorizeAnyType]
        [HttpGet("{deviceId}/property/{propertyName}")]
        public async Task<IActionResult> GetProperty([FromRoute] string deviceId, [FromRoute] string propertyName, [FromHeader(Name = "Token")] string? token)
        {
            DeviceProperty? property = await DB.DeviceProperties
                .Include(p => p.Device)
                .Where(p => p.DeviceId == deviceId && p.PropertyName == propertyName)
                .SingleOrDefaultAsync();

            if (property is null || propertyName is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{deviceId}.{propertyName}' not found"));
            }

            if (token is null && property.Device.OwnerId != AuthorizedUserId)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Access is denied"));
            }
            else if (token is not null && property.Device.Token != token)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Access is denied"));
            }

            return Ok(property.Value);
        }

        [AuthorizeAnyType]
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await DB.Devices.Where(d => d.OwnerId == AuthorizedUserId).ToListAsync();
            return Ok(new DeviceResponse(devices));
        }
    }
}
