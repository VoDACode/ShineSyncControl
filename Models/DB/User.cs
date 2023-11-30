using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsActivated { get; set; } = false;
        public string? ActivationCode { get; set; }

        public string Role { get; set; }
        public UserRoles RoleEntity { get; set; }


        public ICollection<ActionModel> Actions { get; set; } = new List<ActionModel>();
        public ICollection<UserGroup> Groups { get; set; } = new List<UserGroup>();
        public ICollection<Device> Devices { get; set; } = new List<Device>();
        public ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
    }
}
