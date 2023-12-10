using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class ActionTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ActionId { get; set; }
        public ActionModel Action { get; set; }
        [Required]
        public int WhenTrueTaskId { get; set; }
        public TaskModel WhenTrueTask { get; set; }
        [Required]
        public int WhenFalseTaskId { get; set; }
        public TaskModel WhenFalseTask { get; set; }
    }
}
