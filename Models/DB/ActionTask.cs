using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class ActionTask
    {
        [Required]
        public int ActionId { get; set; }
        public Action Action { get; set; }
        [Required]
        public int WhenTrueTaskId { get; set; }
        public Task WhenTrueTask { get; set; }
        [Required]
        public int WhenFalseTaskId { get; set; }
        public Task WhenFalseTask { get; set; }
    }
}
