using EventPhotographer.App.Events.DTO.Response;

namespace EventPhotographer.App.Events.DTO;

public class ParticipantDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
