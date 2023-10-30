using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class UserDevice
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public Device Device { get; set; }

        public bool CanSetProperty { get; set; } = false;
        public bool CanRename { get; set; } = false;
    }
}
