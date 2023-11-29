using System.ComponentModel.DataAnnotations;

namespace ShineSyncControl.Models.Requests
{
    public class ActionPostRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public ExpressionPostRequest Expression { get; set; }

        [Required]
        public TaskPostRequest Task { get; set; }
    }
}
