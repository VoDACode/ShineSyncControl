using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class ExpressionPostRequest
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string DeviceProperty { get; set; }
        [Required]
        public ComparisonOperator Operator { get; set; }
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }

        public ExpressionPostRequest? SubExpression { get; set; }
        public LogicalOperator? ExpressionOperator { get; set; }
    }
}
