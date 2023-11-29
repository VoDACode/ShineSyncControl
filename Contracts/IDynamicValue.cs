using ShineSyncControl.Enums;

namespace ShineSyncControl.Contracts
{
    public interface IDynamicValue
    {
        string Value { get; set; }
        PropertyType Type { get; set; }
    }
}
