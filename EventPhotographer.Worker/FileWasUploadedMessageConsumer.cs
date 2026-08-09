using Amazon.S3.Model;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Content.Services;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Worker.Consumers;

internal class FileWasUploadedMessageConsumer(
    AppDbContext db,
    IDistributedLockProvider lockProvider,
    MediaStorageService storageService,
    FileContentTypeReader fileContentTypeReader,
    IBus bus) 
    : IConsumeAsync<FileWasUploadedMessage>
{
    public async Task ConsumeAsync(FileWasUploadedMessage message, CancellationToken cancellationToken = default)
    {
        MediaFile? mediaFile = await GetMediaFileForProcessingAsync(message.MediaFileId, cancellationToken);
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
                await db.SaveChangesAsync(cancellationToken);
                return;
            }

            mediaFile.Media.Status = MediaStatus.NeedsValidation;
            await db.SaveChangesAsync(cancellationToken);
            await bus.Scheduler.FuturePublishAsync(message, TimeSpan.FromMinutes(1), cancellationToken);

            return;
        }

        var fileContentType = fileContentTypeReader.DetermineFileExtension(storageResponse.ResponseStream);

        mediaFile.Media.Status = MediaStatus.Invalid;
        if (fileContentType != null && fileContentType.IsAllowed)
        {
            mediaFile.Media.Status = MediaStatus.Validated;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<MediaFile?> GetMediaFileForProcessingAsync(Guid mediaFileId, CancellationToken cancellationToken)
    {
        MediaFile? mediaFile = null;

        await using (var @lock = await lockProvider.AcquireLockAsync(
            $"FileWasUploadedMessage:{mediaFileId}",
            TimeSpan.FromSeconds(15),
            cancellationToken
        ))
        {
            mediaFile = await db.MediaFiles
                .Where(f => f.Id == mediaFileId)
                .Include(f => f.Media)
                .FirstOrDefaultAsync(cancellationToken);

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
            await db.SaveChangesAsync(cancellationToken);
        }

        return mediaFile;
    }
}
