using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.UseCases.Common.Authorization;

internal interface IAuthorizationHandler<TRequirement> where TRequirement : class, IAuthorizationRequirement
{
    public Task<AuthorizationResult> HandleAsync(User user, TRequirement requirement);

    public Task<AuthorizationResult> HandleAsync(TRequirement requirement);
}

internal abstract class AuthorizationHandler<TRequirement>
    : IAuthorizationHandler<TRequirement>
    where TRequirement : class, IAuthorizationRequirement
{
    public virtual async Task<AuthorizationResult> HandleAsync(User user, TRequirement requirement)
    {
        return await HandleAsync(requirement);
    }

    public virtual async Task<AuthorizationResult> HandleAsync(TRequirement requirement)
    {
        return AuthorizationResult.Failure();
    }
}

internal abstract class ResourceAuthorizationHandler<TResource, TRequirement>
    : AuthorizationHandler<TRequirement>
    where TResource : class
    where TRequirement : class, IAuthorizationRequirement
{
    public virtual async Task<AuthorizationResult> HandleAsync(TResource resource, TRequirement requirement)
    {
        return AuthorizationResult.Failure();
    }

    public virtual async Task<AuthorizationResult> HandleAsync(User user, TResource resource, TRequirement requirement)
    {
        return await HandleAsync(resource, requirement);
    }
}