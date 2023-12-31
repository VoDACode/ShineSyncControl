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
            public TaskResponse.View WhenTrueTask { get; set; }
            public TaskResponse.View? WhenFalseTask { get; set; } = null;

            public string? Description { get; set; }
            public string Name { get; set; }

            public View(ActionTask actionTask)
            {
                Id = actionTask.Id;
                WhenTrueTask = new TaskResponse.View(actionTask.WhenTrueTask);
                if (actionTask.WhenFalseTask is not null)
                {
                    WhenFalseTask = new TaskResponse.View(actionTask.WhenFalseTask);
                }
                Name = actionTask.Action.Name;
                Description = actionTask.Action.Description;
            }
        }
    }
}
