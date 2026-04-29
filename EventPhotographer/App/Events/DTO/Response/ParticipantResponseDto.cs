namespace EventPhotographer.App.Events.DTO.Response;

public class ParticipantResponseDto : ParticipantDto
{
    public EventResponseDto Event { get; set; } = null!;
}
