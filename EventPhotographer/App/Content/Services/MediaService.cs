using EventPhotographer.App.Content.DTO;
using EventPhotographer.Data.Entities.Content;
using EventPhotographer.Data.Entities.Events;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.App.Content.Services;

public class MediaService(
    AppDbContext dbContext,
    EventService eventService,
    MediaStorageService mediaStorageService,
    FileContentTypeReader fileContentTypeReader)
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

    public async Task<MediaFile?> GetFileByIdAsync(Guid id)
    {
        return await dbContext.MediaFiles
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<MediaFile> UploadFile(Media media, IFormFile file)
    {
        var ext = fileContentTypeReader.DetermineFileExtension(file) ?? "";
        var path = await mediaStorageService.UploadFile(
            file, 
            Guid.NewGuid().ToString() + ext
        );

        var mediaFile = new MediaFile
        {
            Media = media,
            MimeType = FileContentTypeReader.GetMimeTypeFromExtension(ext) ?? throw new BadHttpRequestException("Unsupported Mime Type"),
            Path = path,
            FileSize = (ulong)file.Length,
        };

        await dbContext.AddAsync(mediaFile);
        await dbContext.SaveChangesAsync();

        return mediaFile;
    }

    public async Task<IEnumerable<EventMediaResponseDto>> GetForEventAsync(Event @event)
    {
        return await dbContext.Media
            .Where(m => m.EventId == @event.Id)
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

    public async Task<bool> HasFileAsync(Media media)
    {
        return await dbContext.MediaFiles
            .Where(m => m.Id == media.Id)
            .AnyAsync();
    }

    public async Task<Media?> GetByUploadTokenAsync(Guid uploadToken)
    {
        return await dbContext.Media
            .Where(m => m.UploadToken == uploadToken)
            .FirstOrDefaultAsync();
    }
}
