namespace EventPhotographer.App.Events.DTO.Response;

public class EventShareableLinkResponseDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public Guid EventId { get; set; }
}
