using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.UseCases.Common.Authorization;

internal interface IAuthorizationUserAware
{
    public User User { get; }
}
