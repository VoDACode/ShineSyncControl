using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShineSyncControl.Models.Requests
{
    public class ActionPostRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        [JsonPropertyName("expression")]
        public ExpressionPostRequest Expression { get; set; }

        [Required]
        public int WhenTrueTaskId { get; set; }
        public int? WhenFalseTaskId { get; set; }
    }
}
