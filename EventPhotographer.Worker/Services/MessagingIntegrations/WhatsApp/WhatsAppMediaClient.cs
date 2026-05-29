using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;

internal class WhatsAppMediaClient(HttpClient httpClient)
{
    public async Task<Stream> DownloadMediaAsync(WhatsAppMedia media)
    {
        var response = await httpClient.GetAsync(media.Url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }
}
