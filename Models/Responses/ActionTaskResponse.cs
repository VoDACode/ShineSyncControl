using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Models.Responses
{
    public class ActionTaskResponse : BaseResponse
    {
        public ActionTaskResponse(ActionTask actionTask) : base(true, new View(actionTask))
        {
        }

        public ActionTaskResponse(IEnumerable<ActionTask> actionTasks) : base(true, actionTasks.Select(actionTask => new View(actionTask)))
        {
        }

        public class View
        {
            public int Id { get; set; }
            public int ActionId { get; set; }
            public int WhenTrueTaskId { get; set; }
            public int WhenFalseTaskId { get; set; }

            public string? Description { get; set; }
            public string Name { get; set; }

            public View(ActionTask actionTask)
            {
                Id = actionTask.Id;
                ActionId = actionTask.ActionId;
                WhenFalseTaskId = actionTask.WhenFalseTaskId;
                WhenTrueTaskId = actionTask.WhenTrueTaskId;
                Name = actionTask.Action.Name;
                Description = actionTask.Action.Description;
            }
        }
    }
}
