using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class ActionModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int ExpressionId { get; set; }
        public Expression Expression { get; set; }
        public string? Description { get; set; }

        public User Owner { get; set; }
        [Required]
        public int OwnerId { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        // ?
        public ICollection<ActionTask> ActionTasks { get; set; } = new List<ActionTask>();
    }
}
