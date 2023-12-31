using Microsoft.AspNetCore.Mvc;
using ShineSyncControl.Enums;

namespace ShineSyncControl.Models.Requests
{
    public class GetDevicePropertiesRequest
    {
        [FromQuery] 
        public PropertyType? FilterType { get; set; }
        [FromQuery]
        public bool? CanBeEdited { get; set; }
    }
}
