using ShineSyncControl.Enums;
using ShineSyncControl.Models.DB;
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
        public long DevicePropertyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Event { get; set; } // set
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        [Required]
        public PropertyType Type { get; set; }
        public string? Description { get; set; }
    }
}
