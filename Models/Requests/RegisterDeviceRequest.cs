using ShineSyncControl.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class RegisterDeviceRequest
    {
        [Required]
        [MaxLength(100)]
        public string? Type { get; set; }
        [Required]
        public ICollection<RegisterDevicePropertyRequest> Properties { get; set; }
    }
}
