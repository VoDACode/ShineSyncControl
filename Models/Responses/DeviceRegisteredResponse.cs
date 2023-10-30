using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class DeviceRegisteredResponse : BaseResponse
    {
        public DeviceRegisteredResponse(Device device) : base(true, null)
        {
            Data = new View
            {
                Id = device.Id,
                Token = device.Token,
                Type = device.Type,
                RegisteredAt = device.RegisteredAt
            };
        }

        public class View
        {
            public string Id { get; set; }
            public string Token { get; set; }
            public string? Type { get; set; } = null;
            public DateTime RegisteredAt { get; set; }
        }
    }
}
