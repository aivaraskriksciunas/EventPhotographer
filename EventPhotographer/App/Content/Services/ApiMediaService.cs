using EventPhotographer.App.Content.DTO;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Core;
using Microsoft.EntityFrameworkCore;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.App.Content.Services;

public class ApiMediaService : Core.Features.Content.Services.MediaService
{
    private readonly ApiEventService eventService;

    public ApiMediaService(
        AppDbContext dbContext,
        ApiEventService eventService,
        MediaStorageService mediaStorageService,
        FileContentTypeReader fileContentTypeReader) : base(dbContext, fileContentTypeReader, mediaStorageService)
    {
        this.eventService = eventService;
    }

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

        return await base.CreateMedia(participant);
    }

    public async Task<IEnumerable<EventMediaResponseDto>> GetForEventAsync(Event @event)
    {
        return await dbContext.Media
            .Where(m => m.EventId == @event.Id)
            .Where(m => m.Type == MediaType.UserUpload)
            .Where(m => m.Files.Any())
            .Include(m => m.Files)
            .Include(m => m.Participant)
            .Select(m => new EventMediaResponseDto
            {
                Id = m.Id,
                CreatedAt = m.CreatedAt,
                Participant = m.Participant != null ? new Events.DTO.ParticipantDto
                {
                    Id = m.Participant.Id,
                    CreatedAt = m.Participant.CreatedAt,
                    Name = m.Participant.Name,
                } : null,
                Files = m.Files.Select(f => new EventMediaFileResponseDto
                {
                    Id = f.Id,
                    MimeType = f.MimeType,
                    FileSize = f.FileSize,
                }).ToList(),
            })
            .ToListAsync();
    }

    public async Task<EventMediaResponseDto?> GetArchiveForEventAsync(Event @event)
    {
        return await dbContext.Media
            .Where(m => m.EventId == @event.Id)
            .Where(m => m.Type == MediaType.Archive)
            .Include(m => m.Files)
            .Include(m => m.Participant)
            .Select(m => new EventMediaResponseDto
            {
                Id = m.Id,
                CreatedAt = m.CreatedAt,
                Participant = m.Participant != null ? new Events.DTO.ParticipantDto
                {
                    Id = m.Participant.Id,
                    CreatedAt = m.Participant.CreatedAt,
                    Name = m.Participant.Name,
                } : null,
                Files = m.Files.Select(f => new EventMediaFileResponseDto
                {
                    Id = f.Id,
                    MimeType = f.MimeType,
                    FileSize = f.FileSize,
                }).ToList(),
            })
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
