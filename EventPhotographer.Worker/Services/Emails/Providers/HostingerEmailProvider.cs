using EventPhotographer.Core.Features.Emails.Entities;
using EventPhotographer.Worker.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace EventPhotographer.Worker.Services.Emails.Providers;

internal class HostingerEmailProvider(
    HttpClient client,
    IOptions<HostingerEmailConfiguration> _options) : IEmailSender
{
    private readonly HostingerEmailConfiguration options = _options.Value;

    public async Task SendAsync(Email email)
    {
        var payload = new
        {
            To = new string[] { email.Recipient },
            DisplayName = options.SenderDisplayName,
            Subject = email.Subject,
            Text = Regex.Replace(email.Body, "<.*?>", string.Empty),
            Html = email.Body,
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"mailboxes/{options.MailboxResourceId}/send")
            {
                Content = JsonContent.Create(payload)
            };

        var postResponse = await client.SendAsync(request);

        if (!postResponse.IsSuccessStatusCode)
        {
            var error = await postResponse.Content.ReadAsStringAsync();

            throw new ApplicationException($"Failed to send Hostinger email: {error}");
        }
    }
}
