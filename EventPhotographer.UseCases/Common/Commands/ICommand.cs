using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Common.Commands;

public interface ICommand<out TResult> {}

public interface IUserAwareCommand<out TResult> : ICommand<TResult>
{
    public User User { get; init; }
}