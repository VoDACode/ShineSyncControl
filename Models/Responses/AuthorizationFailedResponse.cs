namespace ShineSyncControl.Models.Responses
{
    public class AuthorizationFailedResponse : BaseResponse
    {
        public AuthorizationFailedResponse() : base(
            false,
            "Invalid login or password"
            )
        {
        }
    }
}
