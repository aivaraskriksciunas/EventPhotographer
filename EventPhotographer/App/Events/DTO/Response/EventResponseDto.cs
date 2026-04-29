namespace EventPhotographer.App.Events.DTO.Response;

public class EventResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
