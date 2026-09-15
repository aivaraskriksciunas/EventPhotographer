namespace EventPhotographer.Hubs;

using Microsoft.AspNetCore.SignalR;
using EventPhotographer.Core.Attributes;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Content.Entities;

public class UploadCompletedNotificationPayload
{
    public Guid MediaId { get; set; }
    public MediaStatus Status { get; set; }
}

public class ThumbnailGeneratedNotificationPayload
{
    public Guid MediaId { get; set; }
    public Guid ThumbnailFileId { get; set; }
}

public interface IUploadNotificationClient
{
    Task ReceiveUploadCompletedNotification(UploadCompletedNotificationPayload payload);

    Task ReceiveThumbnailGeneratedNotification(ThumbnailGeneratedNotificationPayload payload);
}

[ActiveParticipantRequired]
public sealed class UploadNotificationHub : Hub<IUploadNotificationClient> 
{
    public async Task SubscribeToUploadNotifications()
    {
        HttpContext? httpContext = Context.GetHttpContext();
        var participant = httpContext?.GetParticipant();
        if (participant == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }        

        await Groups.AddToGroupAsync(Context.ConnectionId, "participant-" + participant.Id.ToString());
    }
}
