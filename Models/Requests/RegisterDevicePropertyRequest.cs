using ShineSyncControl.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class RegisterDevicePropertyRequest
    {
        [Required]
        [MaxLength(50)]
        public string PropertyName { get; set; }
        public bool IsReadOnly { get; set; }
        [Required]
        public PropertyType Type { get; set; }
        [MaxLength(50)]
        public string? PropertyUnit { get; set; }
    }
}
