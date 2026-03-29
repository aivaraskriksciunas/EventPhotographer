namespace EventPhotographer.App.Events.DTO;

public class EventMediaResponseDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<EventMediaFileResponseDto> Files { get; set; } = new List<EventMediaFileResponseDto>();
}
