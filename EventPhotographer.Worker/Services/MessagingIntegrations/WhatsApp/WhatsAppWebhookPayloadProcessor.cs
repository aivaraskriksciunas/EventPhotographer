using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using EventPhotographer.Core.Features.MessagingIntegrations.Exceptions;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp.MessageContentProcessors;
using System.Text.Json;

namespace EventPhotographer.Worker.Services.MessagingIntegrations.WhatsApp;

internal class WhatsAppWebhookPayloadProcessor(
    WhatsAppContactService contactService,
    WhatsAppMessageService messageService,
    IEnumerable<IMessageContentProcessor> contentProcessors)
{
    public async Task HandlePayload(JsonDocument payload)
    {
        try
        {
            var entries = payload.RootElement.GetProperty("entry");
            foreach (var entry in entries.EnumerateArray())
            {
                var changes = entry.GetProperty("changes");
                await ProcessChanges(changes);
            }
        }
        catch (InvalidOperationException) { }
        catch (KeyNotFoundException) { }
    }

    private async Task ProcessChanges(JsonElement changes)
    {
        foreach (var change in changes.EnumerateArray())
        {
            try
            {
                var type = change.GetProperty("field").GetString();
                if (type != "messages")
                {
                    continue;
                }

                await ProcessChange(change.GetProperty("value"));
            }
            catch (InvalidOperationException) { }
            catch (KeyNotFoundException) { }
        }
    }

    private async Task ProcessChange(JsonElement change)
    {
        // Process contacts, storing them in a dictionary for easy access by ID
        var contacts = new Dictionary<string, WhatsAppContact>();

        if (change.TryGetProperty("contacts", out var jsonContacts))
        {
            foreach (var jsonContact in jsonContacts.EnumerateArray())
            {
                var contact = await ProcessContact(jsonContact);
                if (contact == null)
                {
                    continue;
                }

                contacts.Add(contact.WhatsAppId, contact);
                if (contact.WhatsAppUserId != null)
                {
                    // Optionally store by user_id, in case it is provided
                    contacts.Add(contact.WhatsAppUserId, contact);
                }
            }
        }

        if (change.TryGetProperty("messages", out var jsonMessages))
        {
            foreach (var jsonMessage in jsonMessages.EnumerateArray())
            {
                var message = await ProcessMessage(jsonMessage, contacts);
                if (message == null) continue;

                await ProcessMessageContentAsync(message, jsonMessage);
            }
        }
    }

    private async Task<WhatsAppContact?> ProcessContact(JsonElement contact)
    {
        try
        {
            var profileName = contact.GetProperty("profile").GetProperty("name").GetString();
            var whatsAppId = contact.GetProperty("wa_id").GetString();
            string? whatsAppUserId = TryGetString(contact, "user_id");
            if (whatsAppId == null || profileName == null)
            {
                return null;
            }

            return await contactService.UpdateOrCreateAsync(new WhatsAppContact
            {
                WhatsAppId = whatsAppId,
                WhatsAppUserId = whatsAppUserId,
                ProfileName = profileName,
            });
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private async Task<WhatsAppMessage?> ProcessMessage(JsonElement message, Dictionary<string, WhatsAppContact> fetchedContacts)
    {
        var id = message.GetProperty("id").GetString() ?? throw new WhatsAppWebhookException("Message ID was not provided");
        if (await messageService.MessageIdExistsAsync(id))
        {
            return null;
        }

        var from = message.GetProperty("from").GetString()?.Replace("+", "");
        var fromPhoneNumber = message.GetProperty("from").GetString() ?? throw new WhatsAppWebhookException("From number was not provided");
        var type = message.GetProperty("type").GetString() ?? throw new WhatsAppWebhookException("Message type was not provided");
        string? fromUserId = TryGetString(message, "from_user_id");

        WhatsAppContact? contact = null;
        if (fromUserId != null && fetchedContacts.ContainsKey(fromUserId))
            contact = fetchedContacts[fromUserId];
        else if (from != null && fetchedContacts.ContainsKey(from))
            contact = fetchedContacts[from];
        else if (from != null)
            contact = await contactService.GetByWhatsAppIdAsync(from);

        if (contact == null)
        {
            throw new WhatsAppWebhookException($"Could not find contact for message with id {id} and from {from}");
        }

        if (!fromPhoneNumber.StartsWith("+"))
        {
            fromPhoneNumber = "+" + fromPhoneNumber;
        }

        return await messageService.CreateAsync(new WhatsAppMessage
        {
            WhatsAppId = id,
            Type = type,
            PhoneNumber = fromPhoneNumber,
            ReceivedAt = DateTime.UtcNow,
            WhatsAppContact = contact,
        });
    }

    private string? TryGetString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.GetString();
        }

        return null;
    }

    private async Task ProcessMessageContentAsync(WhatsAppMessage message, JsonElement json)
    {
        var processor = contentProcessors.FirstOrDefault(p => p.Supports(message));

        if (processor != null)
        {
            await processor.ProcessMessageContentAsync(message, json);
        }
    }
}
