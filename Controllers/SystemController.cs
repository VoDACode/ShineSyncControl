using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShineSyncControl.Attributes;
using ShineSyncControl.Enums;
using ShineSyncControl.Models.ConfigOptions;
using ShineSyncControl.Models.Responses;

namespace ShineSyncControl.Controllers
{
    [AuthorizeAnyType]
    [Route("api/system")]
    [ApiController]
    public class SystemController : BaseController
    {
        protected readonly DeviceOption deviceOption;
        public SystemController(DbApp db, IOptions<DeviceOption> deviceOption) : base(db)
        {
            this.deviceOption = deviceOption.Value;
        }

        [HttpGet("config/device-activation-timeout")]
        public IActionResult GetDeviceActivationTimeOut()
        {
            return Ok(new BaseResponse.SuccessResponse(deviceOption.ActivationTimeOut));
        }

        [HttpGet("config/device-types")]
        public IActionResult GetDeviceTypes()
        {
            return Ok(new BaseResponse.SuccessResponse(deviceOption.Types));
        }

        [HttpGet("config/property-types")]
        public IActionResult GetPropertyTypes()
        {
            var propertyTypeNames = Enum.GetNames(typeof(PropertyType));
            var propertyTypeValues = Enum.GetValues(typeof(PropertyType));
            var response = Enumerable.Range(0, propertyTypeNames.Length).Select(i =>
            {
                return new
                {
                    Name = propertyTypeNames[i],
                    Value = (int)(propertyTypeValues.GetValue(i) ?? -1)
                };
            });
            return Ok(new BaseResponse.SuccessResponse(response));
        }
    }
}
