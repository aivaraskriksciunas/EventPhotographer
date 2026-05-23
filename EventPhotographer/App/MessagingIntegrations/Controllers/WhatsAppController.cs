using EventPhotographer.App.MessagingIntegrations.Services;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Features.MessagingIntegrations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EventPhotographer.App.MessagingIntegrations.Controllers;


public class WhatsAppController(
    IOptions<WhatsAppConfiguration> config,
    WhatsAppWebhookService webhookService,
    WhatsAppWebhookPayloadLogService payloadService) 
    : ApiController
{
    private readonly WhatsAppConfiguration whatsappConfiguration = config.Value;

    [HttpGet("Webhook")]
    public IActionResult Verify(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.verify_token")] string token,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == whatsappConfiguration.VerificationToken)
        {
            return Ok(challenge);
        }

        return BadRequest();
    }

    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook()
    {
        var bodyReader = new StreamReader(Request.Body);
        var body = await bodyReader.ReadToEndAsync();
        var hash = Request.Headers["X-Hub-Signature-256"].ToString();
        if (await payloadService.WasWebhookAlreadyReceivedAsync(hash))
        {
            return Ok();
        }

        var valid = webhookService.ValidateSignature(hash, body);
        await payloadService.LogWebhookPayload(body, hash, valid);
        if (!valid)
        {
            return BadRequest();
        }

        return Ok();
    }
}
