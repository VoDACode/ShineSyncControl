using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class DeviceResponse : BaseResponse
    {
        public DeviceResponse(Device device) : base(true, null)
        {
            Data = device;
        }

        public DeviceResponse(ICollection<Device> devices) : base(true, null)
        {
            Data = devices.Select(p => new View(p));
        }

        public class View
        {
            public string Id { get; set; }
            public int? OwnerId { get; set; } = null;
            public string? Name { get; set; } = null;
            public string? Type { get; set; } = null;
            public string? Description { get; set; } = null;
            public bool IsActive { get; set; } = false;
            public DateTime? LastSync { get; set; } = null;
            public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
            public DateTime? ActivatedAt { get; set; } = null;

            public View(Device device)
            {
                Id = device.Id;
                OwnerId = device.OwnerId;
                Name = device.Name;
                Type = device.Type;
                Description = device.Description;
                IsActive = device.IsActive;
                LastSync = device.LastSync;
                RegisteredAt = device.RegisteredAt;
                ActivatedAt = device.ActivatedAt;
            }
        }
    }
}
