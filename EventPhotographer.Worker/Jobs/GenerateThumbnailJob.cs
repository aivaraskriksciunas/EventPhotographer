using Amazon.S3.Model;
using EasyNetQ;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Content.Services;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace EventPhotographer.Worker.Jobs;

internal class GenerateThumbnailJob(
    AppDbContext db,
    MediaStorageService storageService,
    IBus bus)
{
    private const string MimeType = "image/png";

    public async Task ExecuteAsync(Guid mediaFileId)
    {
        MediaFile? mediaFile = mediaFile = await db.MediaFiles
            .Where(f => f.Id == mediaFileId)
            .Include(f => f.Media)
            .FirstOrDefaultAsync();
        if (mediaFile == null)
        {
            return;
        }

        GetObjectResponse storageResponse;
        try
        {
            storageResponse = await storageService.GetFileAsync(mediaFile.Path);
        }
        catch (NoSuchKeyException)
        {
            // File doesn't exist in storage anymore
            return;
        }

        var path = $"thumbnails/{mediaFile.Id}.png";
        ulong fileSize = 0;

        using (var image = await Image.LoadAsync(storageResponse.ResponseStream))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(400, 400),
                Mode = ResizeMode.Max,
                Position = AnchorPositionMode.Center,
            }));

            using var stream = new MemoryStream();
            await image.SaveAsPngAsync(stream);
            fileSize = (ulong)stream.Length;

            await storageService.UploadFile(stream, MimeType, path);
        }

        var thumbnailFile = new MediaFile
        {
            Media = mediaFile.Media,
            MimeType = MimeType,
            FileType = MediaFileType.Thumbnail,
            FileSize = (ulong)fileSize,
            Path = path,
        };

        await db.AddAsync(thumbnailFile);
        await db.SaveChangesAsync();

        await bus.PubSub.PublishAsync(new UploadedFileThumbnailGeneratedMessage
        {
            MediaId = thumbnailFile.MediaId,
            ThumbnailFileId = thumbnailFile.Id,
        });
    }
}
