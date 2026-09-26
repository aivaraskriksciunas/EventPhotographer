namespace EventPhotographer.Worker.Services.Emails;

using EventPhotographer.Core.Features.Emails.Entities;

public interface IEmailSender
{
    public Task SendAsync(Email email);
}
