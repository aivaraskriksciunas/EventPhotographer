using EasyNetQ;
using EventPhotographer.Core.Features.Emails.Entities;
using EventPhotographer.Core.Features.Emails.Messages;
using EventPhotographer.Core.Features.Users.Entities;
using Org.BouncyCastle.Cms;

namespace EventPhotographer.Core.Features.Emails.Services;

public class TemplateEmailService(
    AppDbContext db,
    IBus bus)
{
    public async Task ScheduleAccountVerificationEmail(
        User user, AccountVerification accountVerification)
    {
        // To be improved in the future with email editor
        string verificationUrl = $"https://livealbum/account/verify/{accountVerification.Code}";

        await ScheduleEmail(
            user,
            "Verify your account",
            $@"Hello,<br>
<p>Please <a href=""{verificationUrl}"" target=""_blank"">click here</a> to verify your account.<br>
If the link does not work, copy and paste this into your browser:<br>
{verificationUrl}
</p>

<p>This link will be valid for 24 hours.</p>

<p>Thank you for choosing Live Album!</p>",
            EmailType.EMAIL_VERIFICATION);
    }

    private async Task<Email> ScheduleEmail(User user, string subject, string body, EmailType emailType)
    {
        var email = new Email
        {
            Recipient = user.Email,
            RecipientName = user.Name,
            Subject = subject,
            Body = body,
            User = user,
            EmailType = emailType,
            CreatedAt = DateTime.UtcNow,
            EmailStatus = EmailStatus.SCHEDULED,
        };

        await db.Emails.AddAsync(email);
        await db.SaveChangesAsync();

        await bus.PubSub.PublishAsync(new SendEmailMessage { EmailId = email.Id });

        return email;
    }
}
