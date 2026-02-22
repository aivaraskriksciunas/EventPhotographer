using EventPhotographer.App.Events.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events.Authorization.Handlers;

public abstract class EventAccessHandler<TRequirement> : AuthorizationHandler<TRequirement, Event>
    where TRequirement : IAuthorizationRequirement
{
    protected bool IsOwner(string? userId, Event resource)
    {
        if (string.IsNullOrWhiteSpace(userId)) { return false; }

        return userId == resource.UserId;
    }
}
