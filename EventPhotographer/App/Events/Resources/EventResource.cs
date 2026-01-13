using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.Events.Resources;

public class EventResource
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
