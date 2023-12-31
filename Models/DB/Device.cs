using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class Device
    {
        [Key]
        [MaxLength(64)]
        public string Id { get; set; }
        public int? UserId { get; set; } = null;
        public User? User { get; set;} = null;
        [MaxLength(50)]
        public string? Name { get; set; } = null;
        public string? Type { get; set; } = null;
        [MaxLength(200)]
        public string? Description { get; set; } = null;
        [Required]
        [MaxLength(1024)]
        public string Token { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? LastOnline { get; set; } = null;
        public DateTime? LastSync { get; set; } = null;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActivatedAt { get; set; } = null;

        public ICollection<DeviceProperty> Properties { get; set; } = new List<DeviceProperty>();
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
        public ICollection<DeviceGroup> Groups { get; set; } = new List<DeviceGroup>();
    }
}
