using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [AuthorizeAnyType]
    [Route("api/device/groups")]
    [ApiController]
    public class DeviceGroupController : BaseController
    {
        public DeviceGroupController(DbApp db) : base(db)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<DeviceGroup> groups = await DB.DeviceGroups
                .Where(p => p.OwnerId == AuthorizedUserId)
                .ToListAsync();
            return Ok(new DeviceGroupResponse(groups));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            DeviceGroup? group = await DB.DeviceGroups
                .Include(p => p.Devices)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == AuthorizedUserId);
            if (group == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Group not found"));
            }

            return Ok(new DeviceGroupResponse(group));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeviceGroupPostRequest request)
        {
            DeviceGroup group = new DeviceGroup
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = AuthorizedUserId
            };

            group = (await DB.DeviceGroups.AddAsync(group)).Entity;
            if (request.Devices is not null)
            {
                foreach (var deviceId in request.Devices)
                {
                    var device = await DB.Devices.SingleOrDefaultAsync(p => p.Id == deviceId);
                    if (device == null)
                    {
                        return NotFound(new BaseResponse.ErrorResponse("Device not found"));
                    }
                    group.Devices.Add(device);
                }
            }

            await DB.SaveChangesAsync();

            return Ok(new DeviceGroupResponse(group));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] DeviceGroupPutRequest request)
        {
            DeviceGroup? group = await DB.DeviceGroups
                .Include(p => p.Devices)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == AuthorizedUserId);
            if (group == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Group not found"));
            }

            if (request.Name is not null)
            {
                group.Name = request.Name;
            }

            group.Description = request.Description;

            if (request.Devices is not null)
            {
                foreach (var deviceId in request.Devices)
                {
                    var device = await DB.Devices.SingleOrDefaultAsync(p => p.Id == deviceId);
                    if (device == null || group.Devices.Contains(device))
                    {
                        continue;
                    }
                    if (!group.Devices.Contains(device))
                    {
                        group.Devices.Add(device);
                    }
                }

                for (var i = 0; i < group.Devices.Count; i++)
                {
                    var device = group.Devices.ElementAt(i);
                    if (!request.Devices.Contains(device.Id))
                    {
                        group.Devices.Remove(device);
                        i--;
                    }
                }
            }

            await DB.SaveChangesAsync();

            return Ok(new DeviceGroupResponse(group));
        }

        [HttpPost("{groupId}/{deviceId}")]
        public async Task<IActionResult> AddDeviceToGroup([FromRoute] int groupId, [FromRoute] string deviceId)
        {
            DeviceGroup? group = await DB.DeviceGroups
                .Include(p => p.Devices)
                .FirstOrDefaultAsync(p => p.Id == groupId && p.OwnerId == AuthorizedUserId);
            if (group == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Group not found"));
            }

            Device? device = await DB.Devices
                .FirstOrDefaultAsync(p => p.Id == deviceId && p.UserId == AuthorizedUserId);
            if (device == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device not found"));
            }

            if (group.Devices.Contains(device))
            {
                return BadRequest(new BaseResponse.ErrorResponse("The device exists in this group"));
            }

            group.Devices.Add(device);

            await DB.SaveChangesAsync();

            return Ok(new DeviceGroupResponse(group));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            DeviceGroup? group = await DB.DeviceGroups
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == AuthorizedUserId);
            if (group == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Group not found"));
            }

            DB.DeviceGroups.Remove(group);

            await DB.SaveChangesAsync();

            return Ok(new DeviceGroupResponse(group));
        }

        [HttpDelete("{groupId}/{deviceId}")]
        public async Task<IActionResult> DeleteDeviceToGroup([FromRoute] int groupId, [FromRoute] string deviceId)
        {
            DeviceGroup? group = await DB.DeviceGroups
                .Include(p => p.Devices)
                .FirstOrDefaultAsync(p => p.Id == groupId && p.OwnerId == AuthorizedUserId);
            if (group == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Group not found"));
            }

            Device? device = await DB.Devices
                .FirstOrDefaultAsync(p => p.Id == deviceId && p.UserId == AuthorizedUserId);
            if (device == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device not found"));
            }

            if (!group.Devices.Contains(device))
            {
                return BadRequest(new BaseResponse.ErrorResponse("The device does not exist in this group"));
            }

            group.Devices.Remove(device);

            await DB.SaveChangesAsync();

            return Ok(new DeviceGroupResponse(group));
        }
    }
}
