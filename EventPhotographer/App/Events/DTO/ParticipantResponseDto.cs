namespace EventPhotographer.App.Events.DTO;

public class ParticipantResponseDto
{
    public string Name { get; set; } = string.Empty;

    public EventResponseDto Event { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
