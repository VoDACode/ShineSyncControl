using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class ExpressionPostRequest
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public long DevicePropertyId { get; set; }
        [Required]
        public ComparisonOperator Operator { get; set; }
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }

        public ExpressionPostRequest? SubExpression { get; set; }
        public LogicalOperator? ExpressionOperator { get; set; }
    }
}
