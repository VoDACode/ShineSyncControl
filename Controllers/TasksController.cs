using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : BaseController
    {
        public TasksController(DbApp db) : base(db)
        {
        }

        [AuthorizeAnyType(Type = AuthorizeType.User)]
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await DB.Tasks
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Where(p => p.Device.OwnerId == AuthorizedUserId)
                .ToListAsync();
            return Ok(new TaskResponse(tasks));
        }

        [AuthorizeAnyType(Type = AuthorizeType.User)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await DB.Tasks
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(new TaskResponse(task));
        }
    }
}
