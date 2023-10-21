using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public User Owner { get; set;}
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string? Type { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        [MaxLength(512)]
        public string? Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastSync { get; set; }

        public ICollection<DeviceProperty> Properties { get; set; } = new List<DeviceProperty>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
