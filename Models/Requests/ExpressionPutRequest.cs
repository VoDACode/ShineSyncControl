using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class ExpressionPutRequest
    { 
        public string? DeviceId { get; set; }
        public long? DevicePropertyId { get; set; }
        public ComparisonOperator? Operator { get; set; }
        [MaxLength(255)]
        public string? Value { get; set; }
        public PropertyType? Type { get; set; }

        public ExpressionPostRequest? SubExpression { get; set; }
        public LogicalOperator? ExpressionOperator { get; set; }
    }
}
