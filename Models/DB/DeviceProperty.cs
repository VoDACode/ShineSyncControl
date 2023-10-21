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
        public int Id { get; set; }
        [Required]
        public int DeviceId { get; set; }
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

        public void SetValue(string val)
        {
            if(this.Type != PropertyType.String)
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
            this.Value = val ? "true" : "false";
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
    }
}
