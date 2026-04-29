namespace EventPhotographer.App.Events.DTO.Response;

public class EventMediaResponseDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public ParticipantDto? Participant { get; set; } = null;

    public ICollection<EventMediaFileResponseDto> Files { get; set; } = new List<EventMediaFileResponseDto>();
}
