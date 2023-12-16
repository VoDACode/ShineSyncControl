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
    [Route("api/actions")]
    [ApiController]
    public class ActionsController : BaseController
    {
        protected readonly ITaskEventWorker taskEventWorker;

        public ActionsController(DbApp db, ITaskEventWorker taskEventWorker) : base(db)
        {
            this.taskEventWorker = taskEventWorker;
        }

        [HttpGet("able")]
        public IActionResult GetAble()
        {
            return Ok(new BaseResponse.SuccessResponse(taskEventWorker.Events));
        }

        [AuthorizeAnyType]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await DB.ActionTask
                .Include(p => p.WhenTrueTask)
                .Include(p => p.WhenFalseTask)
                .Include(p => p.Action)
                .Where(p => p.Action.OwnerId == AuthorizedUserId)
                .ToListAsync();

            return Ok(new ActionTaskResponse(events));
        }

        [AuthorizeAnyType]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionPostRequest action)
        {
            var actionModel = new ActionModel()
            {
                Name = action.Name,
                Description = action.Description,
                Owner = AuthorizedUser,
                OwnerId = AuthorizedUserId,
            };

            var taskAction = new ActionTask()
            {
                Action = actionModel
            };

            // create task when true
            var validateWhenTrueTaskResult = await handelTaskPostRequest(action.WhenTrueTask, (task) => taskAction.WhenTrueTask = task);
            if (validateWhenTrueTaskResult is not null)
            {
                return validateWhenTrueTaskResult;
            }

            // create task when false
            var validateWhenFalseTaskResult = await handelTaskPostRequest(action.WhenFalseTask, (task) => taskAction.WhenFalseTask = task);
            if (validateWhenFalseTaskResult is not null)
            {
                return validateWhenFalseTaskResult;
            }

            // create expressions

            ExpressionPostRequest? expressionPost = action.Expression;

            Expression? firstExpression = null;
            Expression? lastExpression = null;

            do
            {
                var property = await DB.DeviceProperties.SingleOrDefaultAsync(p => p.DeviceId == expressionPost.DeviceId && p.Name == expressionPost.DeviceProperty);

                if (property is null)
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Property '{expressionPost.DeviceId}.{expressionPost.DeviceProperty}' not found"));
                }

                var expression = (await DB.Expressions.AddAsync(new Expression()
                {
                    DeviceId = property.DeviceId,
                    DeviceProperty = property,
                    DevicePropertyName = property.Name,
                    Value = expressionPost.Value,
                    Type = expressionPost.Type,
                    Operator = expressionPost.Operator,
                    ExpressionOperator = expressionPost.ExpressionOperator
                })).Entity;
                if(lastExpression is not null)
                {
                    lastExpression.SubExpressionId = expression.SubExpressionId;
                    lastExpression.SubExpression = expression;
                }
                lastExpression = expression;

                if(firstExpression is null)
                    firstExpression = expression;

                await DB.SaveChangesAsync();
            } while ((expressionPost = expressionPost?.SubExpression) is not null);

            actionModel.Expression = firstExpression;

            actionModel = (await DB.Actions.AddAsync(actionModel)).Entity;
            await DB.SaveChangesAsync();

            taskAction = (await DB.ActionTask.AddAsync(taskAction)).Entity;
            await DB.SaveChangesAsync();

            return Ok(new ActionTaskResponse(taskAction));
        }

        [AuthorizeAnyType(Type = AuthorizeType.Cookie | AuthorizeType.JWT)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActionPutRequest action)
        {
            ActionTask? actionTask = await DB.ActionTask
                .Include(p => p.Action).ThenInclude(p => p.Expression).ThenInclude(p => p.SubExpression)
                .Include(p => p.WhenTrueTask)
                .Include(p => p.WhenFalseTask)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (actionTask is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            if (actionTask.Action.OwnerId != AuthorizedUserId)
            {
                return Unauthorized(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            if (action.Name is not null)
            {
                actionTask.Action.Name = action.Name;
            }
            if (action.Description is not null)
            {
                actionTask.Action.Description = action.Description;
            }
            
            if (action.WhenTrueTaskId is not null)
            {
                TaskModel? task = await DB.Tasks.Include(p => p.Device).FirstOrDefaultAsync(p => p.Id == action.WhenTrueTaskId);
                if (task is null)
                {
                    return NotFound(new BaseResponse.ErrorResponse($"Task '{action.WhenTrueTaskId}' not found"));
                }
                if (task.Device.OwnerId != AuthorizedUserId)
                {
                    return Unauthorized(new BaseResponse.ErrorResponse($"Task '{action.WhenTrueTaskId}' not found"));
                }

                actionTask.WhenTrueTask = task;
            }

            if (action.WhenFalseTaskId is not null)
            {
                TaskModel? task = await DB.Tasks.Include(p => p.Device).FirstOrDefaultAsync(p => p.Id == action.WhenFalseTaskId);
                if (task is null)
                {
                    return NotFound(new BaseResponse.ErrorResponse($"Task '{action.WhenFalseTaskId}' not found"));
                }
                if (task.Device.OwnerId != AuthorizedUserId)
                {
                    return Unauthorized(new BaseResponse.ErrorResponse($"Task '{action.WhenFalseTaskId}' not found"));
                }

                actionTask.WhenTrueTask = task;
            }

            await DB.SaveChangesAsync();

            return Ok(new ActionTaskResponse(actionTask));
        }

        private async Task<IActionResult?> handelTaskPostRequest(TaskPostRequest task, Action<TaskModel> action)
        {
            Device? device = await DB.Devices.FirstOrDefaultAsync(p => p.Id == task.DeviceId);
            if (device is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Device '{task.DeviceId}' not found"));
            }

            DeviceProperty? property = await DB.DeviceProperties.FirstOrDefaultAsync(p => p.DeviceId == task.DeviceId && p.Name == task.DevicePropertyName);
            if (property is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{task.DevicePropertyName}' not found"));
            }

            if (!taskEventWorker.Events.Contains(task.Event))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Event '{task.Event}' not found"));
            }

            if (task.Type != property.Type)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid data type. Expected '{Enum.GetName(property.Type)}'"));
            }

            var taskModel = new TaskModel
            {
                Name = task.Name,
                Description = task.Description,
                DeviceId = device.Id,
                DevicePropertyName = task.DevicePropertyName,
                DeviceProperty = property,
                EventName = task.Event,
                Type = task.Type,
                Value = task.Value
            };

            taskModel = (await DB.Tasks.AddAsync(taskModel)).Entity;
            await DB.SaveChangesAsync();
            action(taskModel);

            return null;
        }

        private async Task<IActionResult?> handelTaskPutRequest(TaskPutRequest task, Action<TaskModel> action)
        {
            var taskModel = await DB.Tasks.SingleOrDefaultAsync(p => p.Id == task.Id);
            if (taskModel is null)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Tasks '{task.Id}' not found"));
            }

            if (task.DevicePropertyName is not null && task.DeviceId is not null)
            {
                Device? device = await DB.Devices.FirstOrDefaultAsync(p => p.Id == task.DeviceId);
                if (device is null)
                {
                    return NotFound(new BaseResponse.ErrorResponse($"Device '{task.DeviceId}' not found"));
                }
                taskModel.DeviceId = device.Id;

                DeviceProperty? property = await DB.DeviceProperties.FirstOrDefaultAsync(p => p.DeviceId == task.DeviceId && p.Name == task.DevicePropertyName);
                if (property is null)
                {
                    return NotFound(new BaseResponse.ErrorResponse($"Property '{task.DevicePropertyName}' not found"));
                }

                if(property.DeviceId != device.Id)
                {
                    return NotFound(new BaseResponse.ErrorResponse($"Device '{task.DeviceId}' not found property '{task.DevicePropertyName}'"));
                }

                if (task.Type != property.Type)
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Invalid data type. Expected '{Enum.GetName(property.Type)}'"));
                }

                taskModel.DeviceProperty = property;
                taskModel.DevicePropertyName = property.Name;
            }

            if (!taskEventWorker.Events.Contains(task.Event))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Event '{task.Event}' not found"));
            }

            if(task.Value is not null && task.Type is not null)
            {
                taskModel.Value = task.Value;
                taskModel.Type = task.Type.Value;
            }

            if(task.Description is not null)
            {
                taskModel.Description = task.Description;
            }

            await DB.SaveChangesAsync();
            action(taskModel);
            return null;
        }
    }
}
