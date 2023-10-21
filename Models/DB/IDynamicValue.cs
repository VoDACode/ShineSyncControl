namespace ShineSyncControl.Models.DB
{
    public interface IDynamicValue
    {
        string Value { get; set; }
        PropertyType Type { get; set; }
    }
}
