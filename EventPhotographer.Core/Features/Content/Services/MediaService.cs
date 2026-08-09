using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Features.Content.Services;

public class MediaService
{
    protected readonly AppDbContext dbContext;
    protected readonly FileContentTypeReader fileContentTypeReader;

    public MediaService(
        AppDbContext dbContext,
        FileContentTypeReader fileContentTypeReader,
        MediaStorageService mediaStorageService)
    {
        this.dbContext = dbContext;
        this.fileContentTypeReader = fileContentTypeReader;
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
