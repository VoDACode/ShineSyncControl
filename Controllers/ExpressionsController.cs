using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [AuthorizeAnyType(Type = AuthorizeType.User)]
    [Route("api/expressions")]
    [ApiController]
    public class ExpressionsController : BaseController
    {
        public ExpressionsController(DbApp db) : base(db)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpression([FromRoute] int id)
        {
            var expression = await DB.Expressions
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Include(p => p.SubExpression)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == AuthorizedUserId);
            if (expression == null)
            {
                return NotFound();
            }
            return Ok(new ExpressionResponse(expression));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpression([FromRoute] int id, [FromBody] ExpressionPutRequest request)
        {
            var expression = await DB.Expressions
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Include(p => p.User)
                .Include(p => p.SubExpression)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == AuthorizedUserId);

            if (expression == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Expression not found"));
            }

            string deviceId = request.DeviceId ?? expression.DeviceId;
            string propertyName = request.DeviceProperty ?? expression.DeviceProperty.Name;

            if (request.DeviceId is not null)
            {
                var device = await DB.Devices
                    .SingleOrDefaultAsync(p => p.Id == request.DeviceId && p.UserId == AuthorizedUserId);
                if (device == null)
                {
                    return NotFound(new BaseResponse.ErrorResponse("Device not found"));
                }
                expression.DeviceId = device.Id;
                expression.Device = device;
            }

            if (request.DeviceProperty is not null)
            {
                var deviceProperty = await DB.DeviceProperties
                    .SingleOrDefaultAsync(p => p.Name == request.DeviceProperty && p.DeviceId == deviceId);
                if (deviceProperty == null)
                {
                    return NotFound(new BaseResponse.ErrorResponse("Device property not found"));
                }
                expression.DevicePropertyId = deviceProperty.Id;
                expression.DeviceProperty = deviceProperty;
                expression.Type = deviceProperty.Type;
            }

            expression.Operator = request.Operator ?? expression.Operator;

            if (request.Value is not null)
            {
                if (!DeviceProperty.TryParse(request.Value, expression.Type, out var devicePropertyValue))
                {
                    return BadRequest(new BaseResponse.ErrorResponse($"Invalid value type. Expected '{Enum.GetName(expression.Type)}'."));
                }
                expression.Value = request.Value;
            }

            await DB.SaveChangesAsync();
            return Ok(new ExpressionResponse(expression));
        }

        // Create new expression as a subexpression of an existing expression.
        // If the existing expression has a subexpression, it will be moved to the new subexpression.
        [HttpPost("{id}/createin")]
        public async Task<IActionResult> CreateExpressionIn([FromRoute] int id, [FromBody] ExpressionPostRequest request)
        {
            var expression = await DB.Expressions
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Include(p => p.User)
                .Include(p => p.SubExpression)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == AuthorizedUserId);
            if (expression == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Expression not found"));
            }

            var device = await DB.Devices
                .SingleOrDefaultAsync(p => p.Id == request.DeviceId && p.UserId == AuthorizedUserId);
            if (device == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device not found"));
            }

            var deviceProperty = await DB.DeviceProperties
                .SingleOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Name == request.DeviceProperty);
            if(deviceProperty == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Device property not found"));
            }

            if (!DeviceProperty.TryParse(request.Value, deviceProperty.Type, out var devicePropertyValue))
            {
                return BadRequest(new BaseResponse.ErrorResponse($"Invalid value type. Expected '{Enum.GetName(deviceProperty.Type)}'."));
            }

            var subExpression = new Expression
            {
                DeviceId = device.Id,
                Device = device,
                DevicePropertyId = deviceProperty.Id,
                DeviceProperty = deviceProperty,
                Operator = request.Operator,
                Value = request.Value,
                UserId = AuthorizedUserId,
                Type = deviceProperty.Type,
                SubExpressionId = expression.SubExpressionId,
                ExpressionOperator = request.ExpressionOperator
            };

            subExpression = (await DB.Expressions.AddAsync(subExpression)).Entity;

            expression.SubExpression = subExpression;

            await DB.SaveChangesAsync();
            return Ok(new ExpressionResponse(subExpression));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpression([FromRoute] int id)
        {
            var expression = await DB.Expressions
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .Include(p => p.User)
                .Include(p => p.Actions)
                .Include(p => p.SubExpression)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == AuthorizedUserId);
            if (expression == null)
            {
                return NotFound(new BaseResponse.ErrorResponse("Expression not found"));
            }

            if(expression.Actions.Count > 0)
            {
                return BadRequest(new BaseResponse.ErrorResponse($"You can't remove the first expression on actions [{string.Join(',', expression.Actions.Select(p => p.Id))}]"));
            }

            // change all sub expressions to next level
            var subExpressions = await DB.Expressions
                .Where(p => p.SubExpressionId == expression.Id)
                .ToListAsync();
            foreach (var subExpression in subExpressions)
            {
                subExpression.SubExpressionId = expression.SubExpressionId;
            }

            DB.Expressions.Remove(expression);
            await DB.SaveChangesAsync();
            return Ok(new ExpressionResponse(expression));
        }
    }
}
