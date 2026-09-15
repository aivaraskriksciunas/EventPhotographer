using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Consumers;

internal class UploadedFileNotificationConsumer(
    AppDbContext db,
    IHubContext<UploadNotificationHub, IUploadNotificationClient> hubContext)
    : IConsumeAsync<UploadedFileValidatedMessage>,
    IConsumeAsync<UploadedFileThumbnailGeneratedMessage>
{
    public async Task ConsumeAsync(UploadedFileValidatedMessage message, CancellationToken cancellationToken = default)
    {
        var mediaFile = await db.MediaFiles
            .Where(m => m.Id == message.MediaFileId)
            .Include(m => m.Media)
            .FirstOrDefaultAsync();

        if (mediaFile == null)
        {
            return;
        }

        await hubContext.Clients.Group("participant-" + mediaFile.Media.ParticipantId)
            .ReceiveUploadCompletedNotification(new UploadCompletedNotificationPayload
            {
                MediaId = mediaFile.Media.Id,
                Status = mediaFile.Media.Status ?? MediaStatus.Invalid,
            });
    }

    public async Task ConsumeAsync(UploadedFileThumbnailGeneratedMessage message, CancellationToken cancellationToken = default)
    {
        var mediaFile = await db.MediaFiles
            .Where(m => m.Id == message.ThumbnailFileId)
            .Include(m => m.Media)
            .FirstOrDefaultAsync();

        if (mediaFile == null)
        {
            return;
        }

        await hubContext.Clients.Group("participant-" + mediaFile.Media.ParticipantId)
            .ReceiveThumbnailGeneratedNotification(new ThumbnailGeneratedNotificationPayload
            {
                MediaId = mediaFile.Media.Id,
                ThumbnailFileId = mediaFile.Id,
            });
    }
}
