using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.DB
{
    public class ScheduledTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int TaskId { get; set; }
        public TaskModel Task { get; set; }
        [Required]
        public long Interval { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
