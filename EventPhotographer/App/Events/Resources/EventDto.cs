using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventPhotographer.App.Events.Resources;

public class EventDto
{
    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public string EventDuration { get; set; } = string.Empty;
}
