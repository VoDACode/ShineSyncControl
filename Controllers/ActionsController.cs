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
    [Route("api/actions")]
    [ApiController]
    public class ActionsController : BaseController
    {
        protected readonly ITaskEventWorker taskEventWorker;

        public ActionsController(DbApp db, ITaskEventWorker taskEventWorker) : base(db)
        {
            this.taskEventWorker = taskEventWorker;
        }

        [HttpGet("able-events")]
        public IActionResult GetAble()
        {
            return Ok(new BaseResponse.SuccessResponse(taskEventWorker.Events));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await DB.ActionTask
                .Include(p => p.Action)
                .Where(p => p.Action.UserId == AuthorizedUserId)
                .ToListAsync();

            return Ok(new ActionTaskShortResponse(events));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var action = await DB.ActionTask
                .Include(p => p.WhenTrueTask).ThenInclude(p => p.Device)
                .Include(p => p.WhenTrueTask).ThenInclude(p => p.DeviceProperty)
                .Include(p => p.WhenFalseTask).ThenInclude(p => p.Device)
                .Include(p => p.WhenFalseTask).ThenInclude(p => p.DeviceProperty)
                .Include(p => p.Action)
                .SingleOrDefaultAsync(p => p.Action.UserId == AuthorizedUserId && p.Id == id);
            if (action is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            return Ok(new ActionTaskResponse(action));
        }

        [HttpGet("{id}/expressions")]
        public async Task<IActionResult> GetExpression(int id)
        {
            var action = await DB.ActionTask
                .Include(p => p.Action).ThenInclude(p => p.Expression).ThenInclude(p => p.SubExpression).ThenInclude(p => p.DeviceProperty)
                .Include(p => p.Action).ThenInclude(p => p.Expression).ThenInclude(p => p.DeviceProperty)
                .SingleOrDefaultAsync(p => p.Action.UserId == AuthorizedUserId && p.Id == id);
            if (action is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            return Ok(new ExpressionResponse(action.Action.Expression));
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionPostRequest action)
        {
            var actionModel = new ActionModel()
            {
                Name = action.Name,
                Description = action.Description,
                User = AuthorizedUser,
                UserId = AuthorizedUserId,
            };

            var taskAction = new ActionTask()
            {
                Action = actionModel
            };

            var whenTrueTask = await DB.Tasks
                .Include(p => p.Device).ThenInclude(p => p.Properties)
                .SingleOrDefaultAsync(p => p.Id == action.WhenTrueTaskId && p.UserId == AuthorizedUserId);
            if (whenTrueTask is null)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Task '{action.WhenTrueTaskId}' not found"));
            }
            taskAction.WhenTrueTask = whenTrueTask;


            if (action.WhenFalseTaskId is not null)
            {
                var whenFalseTask = await DB.Tasks
                    .Include(p => p.Device).ThenInclude(p => p.Properties)
                    .SingleOrDefaultAsync(p => p.Id == action.WhenFalseTaskId && p.UserId == AuthorizedUserId);
                if (whenFalseTask is null)
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Task '{action.WhenFalseTaskId}' not found"));
                }
                taskAction.WhenFalseTask = whenFalseTask;
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
                    UserId = AuthorizedUserId,
                    DeviceId = property.DeviceId,
                    DeviceProperty = property,
                    DevicePropertyId = property.Id,
                    Value = expressionPost.Value,
                    Type = property.Type,
                    Operator = expressionPost.Operator,
                    ExpressionOperator = expressionPost.ExpressionOperator
                })).Entity;
                if (lastExpression is not null)
                {
                    lastExpression.SubExpressionId = expression.SubExpressionId;
                    lastExpression.SubExpression = expression;
                }
                lastExpression = expression;

                if (firstExpression is null)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActionPutRequest action)
        {
            ActionTask? actionTask = await DB.ActionTask
                .Include(p => p.Action).ThenInclude(p => p.Expression).ThenInclude(p => p.SubExpression)
                .Include(p => p.WhenTrueTask).ThenInclude(p => p.Device)
                .Include(p => p.WhenTrueTask).ThenInclude(p => p.DeviceProperty)
                .Include(p => p.WhenFalseTask).ThenInclude(p => p.Device)
                .Include(p => p.WhenFalseTask).ThenInclude(p => p.DeviceProperty)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (actionTask is null)
            {
                return NotFound(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            if (actionTask.Action.UserId != AuthorizedUserId)
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
                if (task.Device.UserId != AuthorizedUserId)
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
                if (task.Device.UserId != AuthorizedUserId)
                {
                    return Unauthorized(new BaseResponse.ErrorResponse($"Task '{action.WhenFalseTaskId}' not found"));
                }

                actionTask.WhenFalseTask = task;
            }

            await DB.SaveChangesAsync();

            return Ok(new ActionTaskResponse(actionTask));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
            if (actionTask.Action.UserId != AuthorizedUserId)
            {
                return Unauthorized(new BaseResponse.ErrorResponse($"Action '{id}' not found"));
            }
            DB.Expressions.RemoveRange(actionTask.Action.Expression);
            DB.ActionTask.Remove(actionTask);
            DB.Actions.Remove(actionTask.Action);
            await DB.SaveChangesAsync();
            return Ok(new BaseResponse.SuccessResponse());
        }
    }
}
