namespace ShineSyncControl.Models.Requests
{
    public class ActionPutRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? WhenTrueTaskId { get; set; }
        public long? WhenFalseTaskId { get; set; }
    }
}
