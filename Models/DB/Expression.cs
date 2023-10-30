using ShineSyncControl.Extension;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public enum ExpressionOperator
    {
        And,
        Or
    }

    public class Expression : IDynamicValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        [Required]
        public long DevicePropertyId { get; set; }
        public DeviceProperty DeviceProperty { get; set; }
        [Required]
        public Operator Operator { get; set; }

        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }

        public Expression? SubExpression { get; set; }
        public ExpressionOperator? ExpressionOperator { get; set; }

        public ICollection<Action> Actions = new List<Action>();

        public bool Execute()
        {
            if (SubExpression is null)
            {
                return this.CompareValue(DeviceProperty, Operator);
            }

            if (ExpressionOperator == DB.ExpressionOperator.And)
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
