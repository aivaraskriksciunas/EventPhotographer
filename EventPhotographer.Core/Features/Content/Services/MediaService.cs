using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.Content.Services;

public class MediaService
{
    protected readonly AppDbContext dbContext;
    protected readonly FileContentTypeReader fileContentTypeReader;
    private readonly MediaStorageService mediaStorageService;

    public MediaService(
        AppDbContext dbContext,
        FileContentTypeReader fileContentTypeReader,
        MediaStorageService mediaStorageService)
    {
        this.dbContext = dbContext;
        this.fileContentTypeReader = fileContentTypeReader;
        this.mediaStorageService = mediaStorageService;
    }

    public async Task<Media> CreateArchive(Event @event)
    {
        var media = new Media
        {
            Event = @event,
            Type = MediaType.Archive,
        };

        await dbContext.AddAsync(media);
        await dbContext.SaveChangesAsync();

        return media;
    }

    public async Task<MediaFile> UploadFile(Media media, IFormFile file)
    {
        var ext = fileContentTypeReader.DetermineFileExtension(file) ?? "";
        using var readStream = file.OpenReadStream();

        return await UploadFile(media, readStream, ext);
    }

    public async Task<MediaFile> UploadFile(Media media, Stream file, string extension)
    {
        var mimeType = FileContentTypeReader.GetMimeTypeFromExtension(extension) ?? throw new ArgumentException("Unsupported Mime Type");
        var fileLength = (ulong)file.Length;
        var path = await mediaStorageService.UploadFile(
            file,
            mimeType,
            Guid.NewGuid().ToString() + extension
        );

        var mediaFile = new MediaFile
        {
            Media = media,
            MimeType = mimeType,
            Path = path,
            FileSize = fileLength,
        };

        await dbContext.AddAsync(mediaFile);
        await dbContext.SaveChangesAsync();

        return mediaFile;
    }

    public async Task<MediaFile?> GetFileByIdAsync(Guid id)
    {
        return await dbContext.MediaFiles
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
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
