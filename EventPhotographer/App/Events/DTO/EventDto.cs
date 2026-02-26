namespace EventPhotographer.App.Events.DTO;

public class EventDto
{
    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public string EventDuration { get; set; } = string.Empty;
}
