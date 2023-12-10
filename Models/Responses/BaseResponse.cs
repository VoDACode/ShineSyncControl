using System.Text.Json.Serialization;

namespace ShineSyncControl.Models.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        public BaseResponse(bool success, string? message = null, object? data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public BaseResponse(bool success, string? message = null)
        {
            Success = success;
            Message = message;
            Data = null;
        }

        public BaseResponse(bool success, object? data = null)
        {
            Success = success;
            Message = null;
            Data = data;
        }

        public class SuccessResponse : BaseResponse
        {
            public SuccessResponse(string? message = null, object? data = null) : base(true, message, data)
            {
            }
            public SuccessResponse(object? data = null) : base(true, null, data)
            {
            }

            public SuccessResponse() : base(true, null, null)
            {
            }
        }

        public class ErrorResponse : BaseResponse
        {
            public ErrorResponse(string? message = null, object? data = null) : base(false, message, data)
            {
            }
        }
    }
}
