using EventPhotographer.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EventPhotographer.App.MessagingIntegrations.Controllers;


public class WhatsAppController(
    IOptions<WhatsAppConfiguration> config) 
    : ApiController
{
    private readonly WhatsAppConfiguration whatsappConfiguration = config.Value;

    [HttpGet("Webhook")]
    public IActionResult VerifyAction(
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
}
