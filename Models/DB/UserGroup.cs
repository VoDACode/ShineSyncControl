using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class UserGroup
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        public bool CanSetDeviceProperty { get; set; } = false;
        public bool CanRenameDevice { get; set; } = false;
        public bool CanRenameGroup { get; set; } = false;
        public bool CanAddDevice { get; set; } = false;
        public bool CanDeleteDevice { get; set; } = false;
        public bool CanCreateAction { get; set; } = false;
        public bool CanDeleteAction { get; set; } = false;
        public bool CanExecAction { get; set; } = false;
    }
}
