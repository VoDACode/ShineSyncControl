using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class TaskPostRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string DevicePropertyName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Event { get; set; } // set
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }

        public long? Interval { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
