namespace EventPhotographer.Worker.Services.Emails.Providers;

using EventPhotographer.Core.Features.Emails.Entities;
using EventPhotographer.Worker.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Cms;
using System.Text.RegularExpressions;

internal class SmtpEmailProvider(
    IOptions<SmtpConfiguration> _options) : IEmailSender
{
    private readonly SmtpConfiguration options = _options.Value;

    public async Task SendAsync(Email email)
    {
        using var smtp = new SmtpClient();
        
        try
        {
            await smtp.ConnectAsync(
                options.Host,
                options.Port,
                options.Tls ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.None
            );

            if (options.Username != null && options.Password != null)
            {
                await smtp.AuthenticateAsync(options.Username, options.Password);
            }

            var msg = new MimeMessage();
            msg.To.Add(new MailboxAddress(email.RecipientName, email.Recipient));
            msg.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
            msg.Subject = email.Subject;

            var bb = new BodyBuilder();
            bb.HtmlBody = email.Body;
            bb.TextBody = Regex.Replace(email.Body, "<.*?>", string.Empty);
            msg.Body = bb.ToMessageBody();

            await smtp.SendAsync(msg);
        }
        finally
        {
            if (smtp.IsConnected)
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
