using EventPhotographer.App.Events.Services;
using System.Security.Claims;

namespace EventPhotographer.Core.Middleware;

public class ParticipantMiddleware(
    ParticipantService participantService)
    : IMiddleware
{
    public const string HTTP_COOKIE_NAME = "X-Participant";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Guid participantToken;
        if (Guid.TryParse(context.Request.Cookies[HTTP_COOKIE_NAME], out participantToken))
        {
            var participant = await participantService.GetParticipantAsync(participantToken);

            if (participant != null)
            {
                context.SetParticipant(participant);
            }
            else
            {
                context.Response.Cookies.Delete(HTTP_COOKIE_NAME);
            }
        }

        await next(context);
    }
}
