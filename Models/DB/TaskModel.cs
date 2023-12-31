using ShineSyncControl.Contracts;
using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class TaskModel : IDynamicValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(64)]
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        [Required]
        public string DevicePropertyId { get; set; }
        public DeviceProperty DeviceProperty { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(50)]
        public string EventName { get; set; }
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }
        public string? Description { get; set; }

        public long? Interval { get; set; }
        public bool Enabled { get; set; }

        public ICollection<ActionTask> ActionTasks { get; set; } = new List<ActionTask>();
    }
}
