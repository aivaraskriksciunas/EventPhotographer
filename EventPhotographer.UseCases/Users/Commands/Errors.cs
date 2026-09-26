namespace EventPhotographer.UseCases.Users.Commands;

using EventPhotographer.UseCases.Common;

public record AccountVerificationNotAvailableError(DateTime RetryAt)
    : ErrorWithPayload<DateTime>("AccountVerificationNotAvailable", RetryAt)
{ }

public record AccountAlreadyVerified()
    : Error("AccountAlreadyVerified")
{ }