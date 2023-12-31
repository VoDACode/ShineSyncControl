using ShineSyncControl.Contracts;
using ShineSyncControl.Enums;
using ShineSyncControl.Utilities;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class Expression : IDynamicValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        [Required]
        [MaxLength(128)]
        public string DevicePropertyId { get; set; }
        public DeviceProperty DeviceProperty { get; set; }
        [Required]
        public ComparisonOperator Operator { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }

        public int? SubExpressionId { get; set; }
        public Expression? SubExpression { get; set; }
        public LogicalOperator? ExpressionOperator { get; set; }

        public ICollection<ActionModel> Actions = new List<ActionModel>();

        public bool Execute()
        {
            if (SubExpression is null)
            {
                return this.CompareValue(DeviceProperty, Operator);
            }

            if (ExpressionOperator == LogicalOperator.And)
            {
                return this.CompareValue(DeviceProperty, Operator) && SubExpression.Execute();
            }
            else
            {
                return this.CompareValue(DeviceProperty, Operator) || SubExpression.Execute();
            }
        }
    }
}
