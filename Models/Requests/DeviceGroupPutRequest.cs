using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class DeviceGroupPutRequest
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        public string[]? Devices { get; set; }
    }
}
