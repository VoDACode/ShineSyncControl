using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class DeviceGroup
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
        [StringLength(200)]
        public string? Description { get; set; }
        public User Owner { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
