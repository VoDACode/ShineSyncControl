using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public enum PropertyType
    {
        None = 0,
        String,
        Number,
        Boolean,
        DateTime,
        TimeOnly
    }

    public class DeviceProperty : IDynamicValue
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        [Required]
        [MaxLength(50)]
        public string PropertyName { get; set; }
        public bool IsReadOnly { get; set; }
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }
        [MaxLength(50)]
        public string? PropertyUnit { get; set; }
        public DateTime PropertyLastSync { get; set; }

        public DeviceProperty()
        {
            this.SetDefaultValue();
        }

        public bool TrySetValue(string val)
        {
            switch (this.Type)
            {
                case PropertyType.String:
                    this.Value = val;
                    return true;
                case PropertyType.Number:
                    if(double.TryParse(val, out var num))
                    {
                        this.Value = val;
                        return true;
                    }
                    return false;
                case PropertyType.Boolean:
                    if (val == "0" || val == "1")
                    {
                        this.Value = val;
                        return true;
                    }
                    return false;
                case PropertyType.DateTime:
                    if(DateTime.TryParse(val, out var dateTime))
                    {
                        this.Value = val;
                        return true;    
                    }
                    return false;
                case PropertyType.TimeOnly:
                    if (TimeOnly.TryParse(val, out var timeOnly))
                    {
                        this.Value = val;
                        return true;
                    }
                    return false;
                default:
                    this.Value = "";
                    return false;
            }
        }

        public void SetValue(string val)
        {
            if (this.Type != PropertyType.String)
            {
                throw new InvalidOperationException();
            }
            this.Value = val;
        }

        public void SetValue(double val)
        {
            if (this.Type != PropertyType.Number)
            {
                throw new InvalidOperationException();
            }
            this.Value = val.ToString();
        }

        public void SetValue(bool val)
        {
            if (this.Type != PropertyType.Boolean)
            {
                throw new InvalidOperationException();
            }
            this.Value = val ? "1" : "0";
        }

        public void SetValue(DateTime val)
        {
            if (this.Type != PropertyType.DateTime)
            {
                throw new InvalidOperationException();
            }
            this.Value = val.ToString();
        }

        public void SetValue(TimeOnly val)
        {
            if (this.Type != PropertyType.TimeOnly)
            {
                throw new InvalidOperationException();
            }
            this.Value = val.ToString();
        }

        public void SetDefaultValue()
        {
            switch (this.Type)
            {
                case PropertyType.String:
                    this.Value = "";
                    break;
                case PropertyType.Number:
                    this.Value = "0";
                    break;
                case PropertyType.Boolean:
                    this.Value = "0";
                    break;
                case PropertyType.DateTime:
                    this.Value = DateTime.MinValue.ToString();
                    break;
                case PropertyType.TimeOnly:
                    this.Value = TimeOnly.MinValue.ToString();
                    break;
                default:
                    this.Value = "";
                    break;
            }
        }
    }
}
