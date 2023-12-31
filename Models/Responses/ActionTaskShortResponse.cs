using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class ActionTaskShortResponse : BaseResponse
    {
        public ActionTaskShortResponse(ActionTask actionTask) : base(true, new View(actionTask))
        {
        }

        public ActionTaskShortResponse(IEnumerable<ActionTask> actionTasks) : base(true, actionTasks.Select(actionTask => new View(actionTask)))
        {
        }

        public class View
        {
            public int Id { get; set; }
            public int WhenTrueTaskId { get; set; }
            public int? WhenFalseTaskId { get; set; } = null;

            public string? Description { get; set; }
            public string Name { get; set; }

            public View(ActionTask actionTask)
            {
                Id = actionTask.Id;
                WhenTrueTaskId = actionTask.WhenTrueTaskId;
                WhenFalseTaskId = actionTask.WhenFalseTaskId;
                Name = actionTask.Action.Name;
                Description = actionTask.Action.Description;
            }
        }
    }
}
