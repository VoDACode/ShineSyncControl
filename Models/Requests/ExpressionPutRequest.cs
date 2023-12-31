using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class ExpressionPutRequest
    {
        public int? Id { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceProperty { get; set; }
        public ComparisonOperator? Operator { get; set; }
        [MaxLength(255)]
        public string? Value { get; set; }
    }
}
