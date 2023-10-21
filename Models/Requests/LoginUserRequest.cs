using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class LoginUserRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
