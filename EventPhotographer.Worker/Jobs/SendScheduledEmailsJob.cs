using EventPhotographer.Core;
using EventPhotographer.Core.Features.Emails.Entities;
using EventPhotographer.Worker.Services.Emails;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Worker.Jobs;

internal class SendScheduledEmailsJob(
    AppDbContext db,
    IDistributedLockProvider lockProvider,
    IEmailSender emailSender)
{
    public async Task ExecuteAsync()
    {
        var emails = await ReserveEmailsAsync();

        foreach (var email in emails)
        {
            await SendEmailAsync(email);
        }

        await db.SaveChangesAsync();
    }

    public async Task ExecuteAsync(Guid emailId)
    {
        var emails = await ReserveEmailsAsync(emailId);

        foreach (var email in emails)
        {
            await SendEmailAsync(email);
        }

        await db.SaveChangesAsync();
    }

    private async Task<Email> SendEmailAsync(Email email)
    {
        try
        {
            await emailSender.SendAsync(email);
            email.EmailStatus = EmailStatus.SENT;
            email.SentAt = DateTime.UtcNow;
        }
        catch (Exception e)
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.Contexts["Email"] = new
                {
                    Id = email.Id,
                    Recipient = email.Recipient,
                    RecipientName = email.RecipientName,
                    CreatedAt = email.CreatedAt,
                    Subject = email.Subject,
                    UserId = email.User?.Id,
                    Type = email.EmailType.ToString()
                };
            });
            SentrySdk.CaptureException(e, true);
            email.EmailStatus = EmailStatus.FAILED;
        }

        return email;
    }

    private async Task<Email[]> ReserveEmailsAsync(Guid? emailId = null)
    {
        await using (var @lock = await lockProvider.AcquireLockAsync(
            "SendScheduledEmailsJob",
            TimeSpan.FromSeconds(15)
        ))
        {
            var emails = await db.Emails
                .Where(e => e.EmailStatus == EmailStatus.SCHEDULED)
                .Where(e => emailId == null || e.Id == emailId)
                .Include(e => e.User)
                .OrderBy(e => e.Id)
                .Take(5)
                .ToArrayAsync();

            foreach (var email in emails)
            {
                email.EmailStatus = EmailStatus.SENDING;
            }
            await db.SaveChangesAsync();

            return emails;
        }
    }
}
