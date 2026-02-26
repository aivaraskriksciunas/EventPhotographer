using EventPhotographer.App.Events.Entities;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.Events.DTO;

public class EventShareableLinkResponseDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public Guid EventId { get; set; }
}
