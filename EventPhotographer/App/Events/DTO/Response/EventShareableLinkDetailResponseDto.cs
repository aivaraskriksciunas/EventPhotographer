using EventPhotographer.App.MessagingIntegrations.DTO.Response;

namespace EventPhotographer.App.Events.DTO.Response;

public class EventShareableLinkDetailResponseDto : EventShareableLinkResponseDto
{
    public EventResponseDto? Event { get; set; }
}
