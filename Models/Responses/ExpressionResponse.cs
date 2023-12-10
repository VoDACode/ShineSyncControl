using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class ExpressionResponse : BaseResponse
    {
        public ExpressionResponse(Expression expression) : base(true, new View(expression))
        {
        }

        public ExpressionResponse(IEnumerable<Expression> expressions) : base(true, expressions.Select(p => new View(p)))
        {
        }

        public class View
        {
            public int Id { get; set; }
            public string DeviceId { get; set; }
            public long DevicePropertyId { get; set; }
            public ComparisonOperator Operator { get; set; }
            public string? OperatorName => Enum.GetName(Operator);
            public string Value { get; set; }
            public PropertyType Type { get; set; }
            public string? TypeName => Enum.GetName(Type);
            public View? SubExpression { get; set; }
            public LogicalOperator? ExpressionOperator { get; set; }
            public string? ExpressionOperatorName => ExpressionOperator is null ? null : Enum.GetName(ExpressionOperator.Value);

            public View(Expression expression)
            {
                Id = expression.Id;
                DeviceId = expression.DeviceId;
                DevicePropertyId = expression.DevicePropertyId;
                Operator = expression.Operator;
                Value = expression.Value;
                Type = expression.Type;
                if (expression.SubExpression is not null)
                {
                    SubExpression = new View(expression.SubExpression);
                    ExpressionOperator = expression.ExpressionOperator;
                }
            }
        }
    }
}
