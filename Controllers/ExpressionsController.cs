using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.Requests;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [Route("api/expressions")]
    [ApiController]
    public class ExpressionsController : BaseController
    {
        public ExpressionsController(DbApp db) : base(db)
        {
        }

        [AuthorizeAnyType(Type = AuthorizeType.User)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpression(int id)
        {
            var expression = await DB.Expressions
                .Include(p => p.Device)
                .Include(p => p.DeviceProperty)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (expression == null)
            {
                return NotFound();
            }
            return Ok(new ExpressionResponse(expression));
        }

        [AuthorizeAnyType(Type = AuthorizeType.User)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpression(int id, ExpressionPutRequest request)
        {
            return NoContent();
        }
    }
}
