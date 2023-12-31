using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class DeviceGroupResponse : BaseResponse
    {
        public DeviceGroupResponse(DeviceGroup deviceGroup) : base(true, new View(deviceGroup))
        {
        }

        public DeviceGroupResponse(IEnumerable<DeviceGroup> deviceGroups) : base(true, deviceGroups.Select(p => new View(p)))
        {
        }

        public class View
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public IEnumerable<DeviceResponse.View>? Devices { get; set; }

            public View(DeviceGroup deviceGroup)
            {
                Id = deviceGroup.Id;
                Name = deviceGroup.Name;
                Description = deviceGroup.Description;
                Devices = deviceGroup.Devices is null ? null : deviceGroup.Devices.Select(p => new DeviceResponse.View(p));
            }
        }
    }
}
