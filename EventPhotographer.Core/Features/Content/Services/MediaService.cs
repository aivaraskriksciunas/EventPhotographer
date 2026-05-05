using EventPhotographer.Core;
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
            MimeType = FileContentTypeReader.GetMimeTypeFromExtension(ext) ?? throw new ArgumentException("Unsupported Mime Type"),
            Path = path,
            FileSize = (ulong)file.Length,
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
