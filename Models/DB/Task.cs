using ShineSyncControl.Contracts;
using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class Task : IDynamicValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        [Required]
        public long DevicePropertyId { get; set; }
        public DeviceProperty DeviceProperty { get; set; }

        [Required]
        [MaxLength(50)]
        public string Event { get; set; } // set
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }
        public string? Description { get; set; }

        public void Execute()
        {
            if (Event == "set")
            {
                Console.WriteLine($"Device: [{Device.Name}]; Prop: [{DeviceProperty.PropertyName}]; E:[{Event}] O:[{DeviceProperty.Value}] N:[{Value}]");
                DeviceProperty.Value = Value;
            }
        }
    }
}
