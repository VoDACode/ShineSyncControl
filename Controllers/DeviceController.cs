using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using System.Security.Cryptography;
using Task = System.Threading.Tasks.Task;

namespace ShineSyncControl.Controllers
{
    [Route("api/device")]
    [ApiController]
    public class DeviceController : BaseController
    {

        public DeviceController(DbApp db) : base(db)
        {
        }

        [Authorize(Roles = UserRoles.Registrar)]
        [HttpPost("register")]
        public async Task<IActionResult> RegiaterDevice([FromBody] RegiaterDeviceRequest request)
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
                Token = Convert.ToBase64String(token)
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

        [Authorize]
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

            device.IsClientConnected = true;
            await DB.SaveChangesAsync();

            DateTime connectionStart = DateTime.Now;

            while (await DB.Devices.AnyAsync(d => d.Id == request.DeviceId && !d.IsDeviceConnected))
            {
                if (DateTime.Now > connectionStart + TimeSpan.FromHours(60))
                {
                    device.IsClientConnected = false;
                    await DB.SaveChangesAsync();
                    return BadRequest(new BaseResponse.ErrorResponse("Timeout"));
                }
                await Task.Delay(1000);
            }

            device = await DB.Devices.SingleAsync(d => d.Id == request.DeviceId);

            connectionStart = DateTime.Now;

            while (await DB.Devices.AnyAsync(d => d.Id == request.DeviceId && d.IsDeviceConnected))
            {
                if (DateTime.Now > connectionStart + TimeSpan.FromHours(60))
                {
                    device.IsClientConnected = false;
                    await DB.SaveChangesAsync();
                    return BadRequest(new BaseResponse.ErrorResponse("Timeout"));
                }
                await Task.Delay(1000);
            }

            device.OwnerId = AuthorizedUserId;
            await DB.SaveChangesAsync();

            return Ok(new BaseResponse.SuccessResponse());
        }

        [AuthorizeDevice(OnlyActivated = false)]
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

            device.IsDeviceConnected = true;
            await DB.SaveChangesAsync();

            DateTime connectionStart = DateTime.Now;

            while (await DB.Devices.AnyAsync(d => d.Id == deviceId && d.Token == token && !d.IsClientConnected))
            {
                if (DateTime.Now > connectionStart + TimeSpan.FromHours(60))
                {
                    device.IsDeviceConnected = false;
                    await DB.SaveChangesAsync();
                    return BadRequest(new BaseResponse.ErrorResponse("Timeout"));
                }
                await Task.Delay(1000);
            }
            device = await DB.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);

            var newToken = new byte[512];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newToken);
            }

            device.Token = Convert.ToBase64String(newToken);
            device.IsDeviceConnected = false;
            device.ActivatedAt = DateTime.UtcNow;
            device.IsActive = true;
            await DB.SaveChangesAsync();

            return Ok(device.Token);
        }

        [AuthorizeDevice]
        [HttpPut("property")]
        public async Task<IActionResult> UpdateProperty([FromBody] UpdatePropertyRequest request)
        {
            Device? device = HttpContext.Items["Device"] as Device;
            if (device is null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device is not registered"));
            }

            DeviceProperty? property = await DB.DeviceProperties.FirstOrDefaultAsync(p => p.DeviceId == device.Id && p.PropertyName == request.PropertyName);
            if(property is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{request.PropertyName}' not found"));
            }

            if(!property.TrySetValue(request.Value))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid data type. Expected '{Enum.GetName(property.Type)}'"));
            }

            device.LastSync = DateTime.UtcNow;
            property.PropertyLastSync = DateTime.UtcNow;
            
            await DB.SaveChangesAsync();

            return Ok(new DevicePropertyResponse(property));
        }

        [Authorize]
        [HttpGet("property")]
        public async Task<IActionResult> GetProperty([FromQuery(Name = "id")] string deviceId, [FromQuery(Name = "key")] string propertyName)
        {
            DeviceProperty? property = await DB.DeviceProperties.Include(p => p.Device).Where(p => p.DeviceId == deviceId && p.PropertyName == propertyName).SingleOrDefaultAsync();

            if(property is null || propertyName is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{deviceId}.{propertyName}' not found"));
            }

            if(property.Device.OwnerId != AuthorizedUserId)
            {
                return BadRequest(new BaseResponse.ErrorResponse("Access is denied"));
            }

            return Ok(property.Value);
        }
    }
}
