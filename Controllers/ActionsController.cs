using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
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

        [AuthorizeAnyType]
        [HttpGet]
        public IActionResult Get()
        {
            var events = DB.ActionTask
                .Include(p => p.WhenTrueTask)
                .Include(p => p.WhenFalseTask)
                .Include(p => p.Action).ThenInclude(p => p.Expression).ThenInclude(p => p.SubExpression);

            return Ok(events);
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
                var property = await DB.DeviceProperties.SingleOrDefaultAsync(p => p.Id == expressionPost.DevicePropertyId && p.DeviceId == expressionPost.DeviceId);

                if (property is null)
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Property '{expressionPost.DeviceId}.{expressionPost.DevicePropertyId}' not found"));
                }

                var expression = (await DB.Expressions.AddAsync(new Expression()
                {
                    DeviceId = property.DeviceId,
                    DevicePropertyId = property.Id,
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

            return Ok(taskAction);
        }

        private async Task<IActionResult?> handelTaskPostRequest(TaskPostRequest task, Action<TaskModel> action)
        {
            Device? device = await DB.Devices.FirstOrDefaultAsync(p => p.Id == task.DeviceId);
            if (device is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Device '{task.DeviceId}' not found"));
            }

            DeviceProperty? whenTrueTaskProperty = await DB.DeviceProperties.FirstOrDefaultAsync(p => p.DeviceId == task.DeviceId && p.Id == task.DevicePropertyId);
            if (whenTrueTaskProperty is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Property '{task.DevicePropertyId}' not found"));
            }

            if (!taskEventWorker.Events.Contains(task.Event))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Event '{task.Event}' not found"));
            }

            if (task.Type != whenTrueTaskProperty.Type)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid data type. Expected '{Enum.GetName(whenTrueTaskProperty.Type)}'"));
            }

            var taskModel = new TaskModel
            {
                Name = task.Name,
                Description = task.Description,
                DeviceId = device.Id,
                DevicePropertyId = task.DevicePropertyId,
                EventName = task.Event,
                Type = task.Type,
                Value = task.Value
            };

            taskModel = (await DB.Tasks.AddAsync(taskModel)).Entity;
            await DB.SaveChangesAsync();
            action(taskModel);

            return null;
        }
    }
}
