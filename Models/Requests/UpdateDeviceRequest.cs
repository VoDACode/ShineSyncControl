using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class UpdateDeviceRequest
    {
        [MaxLength(50)]
        public string? Name { get; set; } = null;
        [MaxLength(200)]
        public string? Description { get; set; } = null;
    }
}
