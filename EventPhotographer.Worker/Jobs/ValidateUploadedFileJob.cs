using Amazon.S3.Model;
using EasyNetQ;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Content.Services;
using Hangfire;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Worker.Jobs;

internal class ValidateUploadedFileJob(
    AppDbContext db,
    IDistributedLockProvider lockProvider,
    MediaStorageService storageService,
    FileContentTypeReader fileContentTypeReader,
    IBackgroundJobClient backgroundJobs,
    IBus bus)
{
    public async Task ExecuteAsync(Guid mediaFileId, int attempt = 1)
    {
        MediaFile? mediaFile = await GetMediaFileForProcessingAsync(mediaFileId);
        if (mediaFile == null)
        {
            return;
        }

        GetObjectResponse? storageResponse = null;
        try
        {
            storageResponse = await storageService.GetFileAsync(mediaFile.Path);
        }
        catch (NoSuchKeyException)
        {
            if (mediaFile.Media.CreatedAt < DateTime.Now.Subtract(TimeSpan.FromHours(1)))
            {
                mediaFile.Media.Status = MediaStatus.Invalid;
                await db.SaveChangesAsync();
                return;
            }

            mediaFile.Media.Status = MediaStatus.NeedsValidation;
            await db.SaveChangesAsync();

            // Increase delay on each attempt
            int delay = 1;
            if (attempt > 3 && attempt <= 5) delay = 2;
            else if (attempt > 5) delay = 8;

            backgroundJobs.Schedule<ValidateUploadedFileJob>(
                m => m.ExecuteAsync(mediaFileId, attempt + 1), 
                TimeSpan.FromMinutes(delay)); 

            return;
        }

        var fileContentType = fileContentTypeReader.DetermineFileExtension(storageResponse.ResponseStream);

        mediaFile.Media.Status = MediaStatus.Invalid;
        if (fileContentType != null && fileContentType.IsAllowed)
        {
            mediaFile.Media.Status = MediaStatus.Validated;
        }

        await db.SaveChangesAsync();
        await bus.PubSub.PublishAsync(new UploadedFileValidatedMessage { MediaFileId = mediaFile.Id });

        if (mediaFile.Media.Status == MediaStatus.Validated)
        {
            backgroundJobs.Enqueue<GenerateThumbnailJob>(m => m.ExecuteAsync(mediaFile.Id));
        }
    }

    private async Task<MediaFile?> GetMediaFileForProcessingAsync(Guid mediaFileId)
    {
        MediaFile? mediaFile = null;

        await using (var @lock = await lockProvider.AcquireLockAsync(
            $"ValidateUploadedFileJob:{mediaFileId}",
            TimeSpan.FromSeconds(15)
        ))
        {
            mediaFile = await db.MediaFiles
                .Where(f => f.Id == mediaFileId)
                .Include(f => f.Media)
                .FirstOrDefaultAsync();

            if (mediaFile is null)
            {
                return null;
            }

            if (mediaFile.Media.Status != MediaStatus.NeedsValidation)
            {
                // File has been processed, no need to validate again
                return null;
            }

            mediaFile.Media.Status = MediaStatus.Validating;
            await db.SaveChangesAsync();
        }

        return mediaFile;
    }
}