namespace EventPhotographer.UseCases.Content.Commands;

using EasyNetQ;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.Queries;
using EventPhotographer.UseCases.Content.Authorization;
using EventPhotographer.UseCases.Content.Queries;
using Microsoft.EntityFrameworkCore;

public record ValidateMediaCommand :
    ICommand<Media>
{
    public Guid Id { get; set; }
    public required Participant? Participant { get; set; }
}

internal class ValidateMediaCommandHandler(
    AuthorizationService authorizationService,
    AppDbContext db,
    IBus bus) 
    : ICommandHandler<ValidateMediaCommand, Media>
{
    public async Task<Result<Media>> HandleAsync(
        ValidateMediaCommand command, 
        CancellationToken cancellationToken = default)
    {
        var media = await db.Media
            .Where(m => m.Id == command.Id)
            .Where(m => m.ParticipantId == command.Participant.Id)
            .Include(m => m.Files)
            .FirstOrDefaultAsync();
        if (media == null)
        {
            return Error.NotFound;
        }

        var auth = await authorizationService.AuthorizeAsync(null, media, new ViewMediaRequirement
        {
            Participant = command.Participant
        });
        if (!auth.IsAuthorized)
        {
            return auth;
        }

        if (media.Status == MediaStatus.NeedsValidation)
        {
            await bus.PubSub.PublishAsync(new ValidateUploadedFileMessage { MediaFileId = media.Id });
        }

        return media;
    }
}


