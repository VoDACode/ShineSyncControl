using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class TaskPutRequest
    {
        [Required]
        public long Id { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        public string? DeviceId { get; set; }
        public long? DevicePropertyId { get; set; }
        [MaxLength(50)]
        public string? Event { get; set; } // set
        [MaxLength(255)]
        public string? Value { get; set; }
        public PropertyType? Type { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
