namespace ShineSyncControl.Services.DeviceCommand.Models.Responses
{
    public class CommandTimeResponse : BaseCommandResponse
    {
        public double Time { get; set; }

        public CommandTimeResponse(DateTime time) : base(true)
        {
            Time = time.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        public CommandTimeResponse() : base(true)
        {
            Time = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }
    }
}
