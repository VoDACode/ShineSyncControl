using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class UserUpdateRequest
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
    }
}
