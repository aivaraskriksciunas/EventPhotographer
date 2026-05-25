using EasyNetQ;
using EventPhotographer.App.MessagingIntegrations.Services.WhatsApp;
using EventPhotographer.Core.Features.MessagingIntegrations.Messages;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.MessagingIntegrations.Controllers;


public class WhatsAppController(
    WhatsAppWebhookPayloadService payloadService,
    WhatsAppWebhookService webhookService)
    : ApiController
{
    [HttpGet("Webhook")]
    public IActionResult Verify(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.verify_token")] string token,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && webhookService.IsVerificationTokenValid(token))
        {
            return Ok(challenge);
        }

        return BadRequest();
    }

    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook(
        [FromServices] IBus bus)
    {
        var bodyReader = new StreamReader(Request.Body);
        var body = await bodyReader.ReadToEndAsync();
        var hash = Request.Headers["X-Hub-Signature-256"].ToString();
        if (await payloadService.WasWebhookAlreadyReceivedAsync(hash))
        {
            return Ok();
        }

        var valid = webhookService.ValidateSignature(hash, body);
        var payload = await payloadService.LogWebhookPayloadAsync(body, hash, valid);
        if (!valid)
        {
            // Do not let any possible third parties to know it's not a valid payload
            return Ok();
        }

        await bus.PubSub.PublishAsync(new ProcessWhatsAppWebhookPayload
        {
            WhatsAppPayloadId = payload.Id
        });

        return Ok();
    }
}
