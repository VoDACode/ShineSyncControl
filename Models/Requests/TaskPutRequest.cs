using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class TaskPutRequest
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        public string? DeviceId { get; set; }
        public string? DevicePropertyName { get; set; }
        [MaxLength(50)]
        public string? Event { get; set; }
        [MaxLength(255)]
        public string? Value { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public long? Interval { get; set; }
        public bool? Enabled { get; set; }
    }
}
