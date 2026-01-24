namespace EventPhotographer.App.Events.Resources;

public class AnonymousEventCreatedResponseDto : EventResponseDto
{
    public string AdministratorAccessKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
