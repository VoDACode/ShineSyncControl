using ShineSyncControl.Contracts;
using ShineSyncControl.Enums;
using ShineSyncControl.Exceptions;

namespace ShineSyncControl.Utilities
{
    public static class ValueComparer
    {
        public static bool CompareValue(this IDynamicValue b, IDynamicValue a, ComparisonOperator @operator)
        {
            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return a.EquelsValue(b);
                case ComparisonOperator.NotEqual:
                    return !a.EquelsValue(b);
                case ComparisonOperator.GreaterThan:
                    return a.GreaterThan(b);
                case ComparisonOperator.GreaterThanOrEqual:
                    return a.GreaterThanOrEqual(b);
                case ComparisonOperator.LessThan:
                    return a.LessThan(b);
                case ComparisonOperator.LessThanOrEqual:
                    return a.LessThanOrEqual(b);
            }
            return false;
        }

        public static bool EquelsValue(this IDynamicValue a, IDynamicValue b)
        {
            if (a is null || b is null)
                return true;
            if (a.Type != b.Type || a.Value != b.Value)
                return false;
            return true;
        }

        public static bool GreaterThan(this IDynamicValue a, IDynamicValue b)
        {
            validNullAndType(a, b);

            switch (a.Type)
            {
                case PropertyType.None:
                    return false;
                case PropertyType.String:
                    return a.Value.CompareTo(b.Value) > 0;
                case PropertyType.Number:
                    return double.TryParse(a.Value, out double ad) && double.TryParse(b.Value, out double bd) && ad > bd;
                case PropertyType.Boolean:
                    return a.Value == "1" && b.Value == "0";
                case PropertyType.DateTime:
                    return DateTime.Parse(a.Value) > DateTime.Parse(b.Value);
                case PropertyType.TimeOnly:
                    return TimeOnly.Parse(a.Value) > TimeOnly.Parse(b.Value);
                default:
                    throw new UnknownTypeException();
            }
        }

        public static bool GreaterThanOrEqual(this IDynamicValue a, IDynamicValue b)
        {
            validNullAndType(a, b);

            switch (a.Type)
            {
                case PropertyType.None:
                    return false;
                case PropertyType.String:
                    return a.Value.CompareTo(b.Value) >= 0;
                case PropertyType.Number:
                    return double.TryParse(a.Value, out double ad) && double.TryParse(b.Value, out double bd) && ad >= bd;
                case PropertyType.Boolean:
                    return (a.Value == "1" && b.Value == "0") || a.Value == b.Value;
                case PropertyType.DateTime:
                    return DateTime.Parse(a.Value) >= DateTime.Parse(b.Value);
                case PropertyType.TimeOnly:
                    return TimeOnly.Parse(a.Value) >= TimeOnly.Parse(b.Value);
                default:
                    throw new UnknownTypeException();
            }
        }

        public static bool LessThan(this IDynamicValue a, IDynamicValue b)
        {
            validNullAndType(a, b);

            switch (a.Type)
            {
                case PropertyType.None:
                    return false;
                case PropertyType.String:
                    return a.Value.CompareTo(b.Value) < 0;
                case PropertyType.Number:
                    return double.TryParse(a.Value, out double ad) && double.TryParse(b.Value, out double bd) && ad < bd;
                case PropertyType.Boolean:
                    return a.Value == "1" && b.Value == "0";
                case PropertyType.DateTime:
                    return DateTime.Parse(a.Value) < DateTime.Parse(b.Value);
                case PropertyType.TimeOnly:
                    return TimeOnly.Parse(a.Value) < TimeOnly.Parse(b.Value);
                default:
                    throw new UnknownTypeException();
            }
        }

        public static bool LessThanOrEqual(this IDynamicValue a, IDynamicValue b)
        {
            validNullAndType(a, b);

            switch (a.Type)
            {
                case PropertyType.None:
                    return false;
                case PropertyType.String:
                    return a.Value.CompareTo(b.Value) <= 0;
                case PropertyType.Number:
                    return double.TryParse(a.Value, out double ad) && double.TryParse(b.Value, out double bd) && ad <= bd;
                case PropertyType.Boolean:
                    return (a.Value == "1" && b.Value == "0") || a.Value == b.Value;
                case PropertyType.DateTime:
                    return DateTime.Parse(a.Value) <= DateTime.Parse(b.Value);
                case PropertyType.TimeOnly:
                    return TimeOnly.Parse(a.Value) <= TimeOnly.Parse(b.Value);
                default:
                    throw new UnknownTypeException();
            }
        }

        private static void validNullAndType(IDynamicValue a, IDynamicValue b)
        {
            if (a is null || b is null)
                throw new NullReferenceException();
            if (a.Type != b.Type)
                throw new DifferentTypeException();
        }
    }
}
