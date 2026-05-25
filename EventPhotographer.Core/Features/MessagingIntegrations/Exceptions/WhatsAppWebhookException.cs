namespace EventPhotographer.Core.Features.MessagingIntegrations.Exceptions;

public class WhatsAppWebhookException : Exception
{
    public WhatsAppWebhookException() { }

    public WhatsAppWebhookException(string message) : base(message) { }

    public WhatsAppWebhookException(string message, Exception inner) : base(message, inner) { }
}
