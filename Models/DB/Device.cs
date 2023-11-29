using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class Device
    {
        [Key]
        public string Id { get; set; }
        public int? OwnerId { get; set; } = null;
        public User? Owner { get; set;} = null;
        [MaxLength(50)]
        public string? Name { get; set; } = null;
        public string? Type { get; set; } = null;
        [MaxLength(200)]
        public string? Description { get; set; } = null;
        [Required]
        [MaxLength(1024)]
        public string Token { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? LastSync { get; set; } = null;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActivatedAt { get; set; } = null;

        public ICollection<DeviceProperty> Properties { get; set; } = new List<DeviceProperty>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
