using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.TaskEventWorker;

namespace ShineSyncControl.Controllers
{
    [AuthorizeAnyType(Type = AuthorizeType.User)]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : BaseController
    {
        protected readonly ITaskEventWorker eventWorker;

        public TasksController(DbApp db, ITaskEventWorker eventWorker) : base(db)
        {
            this.eventWorker = eventWorker;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await DB.Tasks
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Where(p => p.Device.UserId == AuthorizedUserId)
                .ToListAsync();
            return Ok(new TaskResponse(tasks));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask([FromRoute] int id)
        {
            var task = await DB.Tasks
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .FirstOrDefaultAsync(p => p.Id == id && p.Device.UserId == AuthorizedUserId);
            if (task == null)
            {
                return NotFound(new BaseResponse.ErrorResponse());
            }
            return Ok(new TaskResponse(task));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskPostRequest request)
        {
            if (!eventWorker.Events.Contains(request.Event))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Unsupported event. Expected: {string.Join(',', eventWorker.Events)}"));
            }

            var device = await DB.Devices.FirstOrDefaultAsync(p => p.Id == request.DeviceId && p.IsActive);
            if (device == null || device.UserId != AuthorizedUserId)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device not found"));
            }

            var deviceProperty = await DB.DeviceProperties.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Name == request.DevicePropertyName);
            if (deviceProperty == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device property not found"));
            }

            if (!DeviceProperty.TryParse(request.Value, deviceProperty.Type, out var devicePropertyValue))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid value type. Expected '{Enum.GetName(deviceProperty.Type)}'."));
            }

            TaskModel task = new TaskModel()
            {
                UserId = AuthorizedUserId,
                Name = request.Name,
                Description = request.Description,
                DeviceId = deviceProperty.DeviceId,
                Device = device,
                DevicePropertyId = deviceProperty.Name,
                DeviceProperty = deviceProperty,
                Enabled = request.Enabled,
                EventName = request.Event,
                Interval = request.Interval,
                Type = deviceProperty.Type,
                Value = request.Value,
            };
            task = (await DB.Tasks.AddAsync(task)).Entity;

            await DB.SaveChangesAsync();

            return Ok(new TaskResponse(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskPutRequest request, [FromRoute] int id)
        {
            var task = await DB.Tasks
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .FirstOrDefaultAsync(p => p.Id == id && p.Device.UserId == AuthorizedUserId);

            if (task is null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Task not found"));
            }

            string deviceId = request.DeviceId ?? task.DeviceId;
            string propertyName = request.DevicePropertyName ?? task.DevicePropertyId;

            if (request.Event is not null && !eventWorker.Events.Contains(request.Event))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Unsupported event. Expected: {string.Join(',', eventWorker.Events)}"));
            }

            if (request.DeviceId is not null)
            {
                var device = await DB.Devices
                    .Include(p => p.Properties)
                    .SingleOrDefaultAsync(p => p.UserId == AuthorizedUserId && p.Id == request.DeviceId);
                if (device is null)
                {
                    return BadRequest(new BaseResponse.ErrorResponse("Device not found"));
                }
                task.DeviceId = device.Id;
                task.Device = device;
            }

            if (request.DevicePropertyName is not null)
            {
                var deviceProperty = await DB.DeviceProperties.SingleOrDefaultAsync(p => p.DeviceId == deviceId && p.Name == propertyName);
                if (deviceProperty is null)
                {
                    return BadRequest(new BaseResponse.ErrorResponse("Property not found"));
                }
                task.DevicePropertyId = deviceProperty.DeviceId;
                task.DeviceProperty = deviceProperty;
            }

            if (request.Value is not null)
            {
                if (!DeviceProperty.TryParse(request.Value, task.Type, out var devicePropertyValue))
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Invalid value type. Expected '{Enum.GetName(task.Type)}'."));
                }
                task.Value = request.Value;
            }

            task.Name = request.Name ?? task.Name;
            task.Description = request.Description ?? task.Description;
            task.Interval = request.Interval ?? task.Interval;
            task.Enabled = request.Enabled ?? task.Enabled;

            await DB.SaveChangesAsync();

            return Ok(new TaskResponse(task));
        }
    }
}
