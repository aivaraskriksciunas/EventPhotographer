using EventPhotographer.App.Content.DTO;
using EventPhotographer.App.Content.Entities;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Core;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.App.Content.Services;

public class MediaService(
    AppDbContext dbContext,
    EventService eventService)
{
    public async Task<Media> CreateMedia(MediaUploadRequestDto uploadRequest, Participant participant)
    {
        var @event = participant.Event;
        if (@event == null)
        {
            @event = await eventService.GetByIdAsync(participant.EventId);
        }

        if (@event == null)
        {
            throw new Exception("Attempted to create media for a participant without an event");
        }

        var media = new Media
        {
            Event = @event,
            Participant = participant,
            UploadToken = Guid.NewGuid(),
        };

        await dbContext.AddAsync(media);
        await dbContext.SaveChangesAsync();

        return media;
    }

    public async Task<Media?> GetByUploadTokenAsync(Guid uploadToken)
    {
        return await dbContext.Media
            .Where(m => m.UploadToken == uploadToken)
            .FirstOrDefaultAsync();
    }
}
