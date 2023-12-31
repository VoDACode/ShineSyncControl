using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Responses;
using ShineSyncControl.Services.TaskEventWorker;

namespace ShineSyncControl.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : BaseController
    {
        protected ITaskEventWorker taskEventWorker;

        public TestController(DbApp db, ITaskEventWorker taskEventWorker) : base(db)
        {
            this.taskEventWorker = taskEventWorker;
        }

        [AuthorizeAnyType(Type = AuthorizeType.JWT | AuthorizeType.Cookie)]
        [HttpGet("jwt")]
        public IActionResult TestJWT()
        {
            return Ok("JWT is working");
        }

        [HttpGet("TaskEventWorker/list")]
        public IActionResult TestTaskEventWorker()
        {
            return Ok(taskEventWorker.Events);
        }

        [HttpGet("device/{deviceId}/{propertyName}")]
        public async Task<IActionResult> TestTaskEventWorkerExecuteAsync(string deviceId, string propertyName, [FromQuery(Name = "val")]string value)
        {
            DeviceProperty? property = await DB.DeviceProperties.Include(p => p.Device).FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.Name == propertyName);
            if (property is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{propertyName}' not found"));
            }

            if (!property.TrySetValue(value))
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
            .Where(a => (a.Action.Expression.DevicePropertyId == property.Id) ||
                        (a.Action.Expression.SubExpression != null && a.Action.Expression.SubExpression.DevicePropertyId == property.Id)
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
    }
}
