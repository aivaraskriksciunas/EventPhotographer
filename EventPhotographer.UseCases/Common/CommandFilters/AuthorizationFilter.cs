using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Common.PipelineBehaviours;

namespace EventPhotographer.UseCases.Common.CommandFilters;

internal class AuthorizationFilter(
    AuthorizationService authorizationService)
    : ICommandFilter
{
    public async Task<Result<TResult>?> ApplyFilterAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResult>
    {
        if (command is not IRequiresAuthorization typedCommand)
        {
            return null;
        }

        var authorizationResult = AuthorizationResult.Failure();
        User? user = null;

        if (command is IAuthorizationUserAware userAwareCommand)
        {
            user = userAwareCommand.User;
        }

        if (command is IRequiresResourceAuthorization resourceAuthCommand)
        {
            authorizationResult = await authorizationService.AuthorizeAsync(
                user, 
                resourceAuthCommand.GetAuthorizationResource(), 
                resourceAuthCommand.GetRequirements());
        }
        else
        {
            authorizationResult = await authorizationService.AuthorizeAsync(
                user,
                typedCommand.GetRequirements());
        }

        if (!authorizationResult.IsAuthorized)
        {
            return Error.AccessDenied;
        }

        return null;
    }
}
