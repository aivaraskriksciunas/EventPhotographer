using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.App.Events.Services;

public static class HttpContextExtensions
{
    public static Participant? GetParticipant(this HttpContext context)
    {
        context.Items.TryGetValue("Participant", out var participant);
        return participant as Participant;
    }

    public static void SetParticipant(this HttpContext context, Participant participant)
    {
        context.Items["Participant"] = participant;
    }
}
