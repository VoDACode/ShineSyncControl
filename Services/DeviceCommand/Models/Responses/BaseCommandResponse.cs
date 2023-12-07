using Newtonsoft.Json;

namespace ShineSyncControl.Services.DeviceCommand.Models.Responses
{
    public abstract class BaseCommandResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error")]
        public string? Error { get; set; }

        public BaseCommandResponse(bool success, string? error = default)
        {
            Success = success;
            Error = error;
        }

        public class SuccessResponse : BaseCommandResponse
        {
            public SuccessResponse() : base(true) { }
        }

        public class ErrorResponse : BaseCommandResponse
        {
            public ErrorResponse() : base(false) { }

            public ErrorResponse(string error) : base(false, error) { }
        }
    }
}
