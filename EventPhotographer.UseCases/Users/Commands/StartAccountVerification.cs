using EventPhotographer.Core.Features.Emails.Services;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Features.Users.Services;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Commands;

namespace EventPhotographer.UseCases.Users.Commands;

public record StartAccountVerification
    : ICommand<StartAccountVerificationResult>
{
    public required User User { get; set; }
}

public record StartAccountVerificationResult
{
    public required AccountVerification AccountVerification { get; init; }

    public required DateTime NextResendDate { get; init; }
}


internal class StartAccountVerificationHandler(
    AccountVerificationService accountVerificationService,
    TemplateEmailService emailService)
    : ICommandHandler<StartAccountVerification, StartAccountVerificationResult>
{
    public async Task<Result<StartAccountVerificationResult>> HandleAsync(
        StartAccountVerification command,
        CancellationToken cancellationToken = default)
    {
        if (command.User.EmailConfirmed)
        {
            return new AccountAlreadyVerified();
        }

        var retryAt = await accountVerificationService
            .CalculateWhenNextVerificationCanBeGenerated(command.User);

        if (retryAt > DateTime.UtcNow)
        {
            return new AccountVerificationNotAvailableError(retryAt);
        }

        var verification = await accountVerificationService.CreateVerification(command.User);
        await emailService.ScheduleAccountVerificationEmail(command.User, verification);

        return new StartAccountVerificationResult
        {
            AccountVerification = verification,
            NextResendDate = await accountVerificationService.CalculateWhenNextVerificationCanBeGenerated(command.User),
        };
    }
}
