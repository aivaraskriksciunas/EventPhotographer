using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.UseCases.Common.Authorization;

public class AuthorizationService(IServiceProvider serviceProvider)
{
    public async Task<AuthorizationResult> AuthorizeAsync<TEntity, TRequirement>(User? user, TEntity entity, TRequirement requirement)
        where TEntity : class, IEntity
        where TRequirement : class, IAuthorizationRequirement
    {
        var handlers = serviceProvider.GetServices(typeof(IAuthorizationHandler<TRequirement>));

        foreach (var handler in handlers)
        {
            if (handler is not IAuthorizationHandler<TRequirement> typedHandler)
                continue;

            var result = AuthorizationResult.Failure();
            if (handler is ResourceAuthorizationHandler<TEntity, TRequirement> resourceAuthorizationHandler)
            {
                if (user == null)
                    result = await resourceAuthorizationHandler.HandleAsync(entity, requirement);
                else
                    result = await resourceAuthorizationHandler.HandleAsync(user, entity, requirement);
            }
            else
            {
                if (user == null)
                    result = await typedHandler.HandleAsync(requirement);
                else
                    result = await typedHandler.HandleAsync(user, requirement);
            }

            if (!result.IsAuthorized)
            {
                return result;
            }
        }

        return AuthorizationResult.Success();
    }

    public async Task<AuthorizationResult> AuthorizeAsync<TRequirement>(User? user, TRequirement requirement)
        where TRequirement : class, IAuthorizationRequirement
    {
        var handlers = serviceProvider.GetServices(typeof(IAuthorizationHandler<TRequirement>));

        foreach (var handler in handlers)
        {
            if (handler is not IAuthorizationHandler<TRequirement> typedHandler)
                continue;

            var result = AuthorizationResult.Failure();
            if (user == null)
                result = await typedHandler.HandleAsync(requirement);
            else
                result = await typedHandler.HandleAsync(user, requirement);

            if (!result.IsAuthorized)
            {
                return result;
            }
        }

        return AuthorizationResult.Success();
    }

    public async Task<AuthorizationResult> AuthorizeAsync(User? user, IEnumerable<IAuthorizationRequirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            var result = await AuthorizeAsync(user, requirement);
            if (!result.IsAuthorized)
            {
                return result;
            }
        }

        return AuthorizationResult.Success();
    }

    public async Task<AuthorizationResult> AuthorizeAsync<TEntity>(User? user, TEntity entity, IEnumerable<IAuthorizationRequirement> requirements)
        where TEntity: class, IEntity
    {
        foreach (var requirement in requirements)
        {
            var result = await AuthorizeAsync(user, entity, requirement);
            if (!result.IsAuthorized)
            {
                return result;
            }
        }

        return AuthorizationResult.Success();
    }
}
