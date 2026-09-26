namespace EventPhotographer.Core.Features.Emails.Messages;

public record SendEmailMessage
{
    public required Guid EmailId { get; init; }
}
