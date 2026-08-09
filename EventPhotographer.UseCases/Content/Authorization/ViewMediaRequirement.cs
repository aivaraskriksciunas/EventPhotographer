using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.Core.Features.Content.Entities;

namespace EventPhotographer.UseCases.Content.Authorization;

public class ViewMediaRequirement : IAuthorizationRequirement 
{
    public Participant? Participant { get; set; } = null;
}

internal class ViewMediaRequirementHandler : ResourceAuthorizationHandler<Media, ViewMediaRequirement>
{
    public override async Task<AuthorizationResult> HandleAsync(Media media, ViewMediaRequirement requirement)
    {
        if (requirement.Participant != null && media.Participant == requirement.Participant)
        {
            return AuthorizationResult.Success();
        }

        return AuthorizationResult.Failure();
    }

    public override async Task<AuthorizationResult> HandleAsync(User user, Media media, ViewMediaRequirement requirement)
    {
        if ((await HandleAsync(media, requirement)).IsAuthorized)
        {
            return AuthorizationResult.Success();
        }

        if (media.Event.User == user)
        {
            return AuthorizationResult.Success();
        }

        return AuthorizationResult.Failure();
    }
}