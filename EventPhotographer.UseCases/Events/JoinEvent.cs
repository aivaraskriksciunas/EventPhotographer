using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Events.Authorization;
using FluentValidation;

namespace EventPhotographer.UseCases.Events;

public record JoinEvent 
    : ICommand<Participant>
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public User? User { get; set; } = null;
}

internal class JoinEventValidator : AbstractValidator<JoinEvent>
{
    public JoinEventValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(100);
    }
}

internal class JoinEventHandler (
    EventShareableLinkService shareableLinkService,
    AuthorizationService authorizationService,
    ParticipantService participantService) 
    : ICommandHandler<JoinEvent, Participant>
{
    public IEnumerable<IAuthorizationRequirement> Requirements => [new JoinEventRequirement()];

    public async Task<Result<Participant>> HandleAsync(JoinEvent command, CancellationToken cancellationToken = default)
    {
        var shareableLink = await shareableLinkService.GetShareableLinkByCode(command.Code);
        if (shareableLink == null)
        {
            return Error.NotFound;
        }

        var authResult = await authorizationService.AuthorizeAsync(command.User, shareableLink.Event, new JoinEventRequirement());
        if (!authResult.IsAuthorized)
        {
            return Error.NotFound;
        }

        return await participantService.CreateOrGetParticipantAsync(
            shareableLink,
            command.Name,
            command.User
        );
    }
}
