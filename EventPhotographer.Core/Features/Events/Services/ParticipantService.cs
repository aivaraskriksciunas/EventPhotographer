using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.App.Events.Services;

public class ParticipantService(
    AppDbContext Db)
{
    public async Task<Participant> CreateOrGetParticipant(
        EventShareableLink shareableLink,
        string name,
        User? user)
    {
        Participant? participant = null;
        if (user != null)
        {
            participant = await GetForUser(shareableLink.Event, user);
            if (participant != null)
            {
                participant.Name = name;
                Db.Participants.Update(participant);
                await Db.SaveChangesAsync();

                return participant;
            }
        }

        participant = new Participant
        {
            Name = name,
            Event = shareableLink.Event,
            EventShareableLink = shareableLink,
            CreatedAt = DateTime.UtcNow,
            User = user,
        };

        await Db.Participants.AddAsync(participant);
        await Db.SaveChangesAsync();

        return participant;
    }

    public async Task<Participant?> GetForUser(
        Event @event,
        User user)
    {
        return await Db.Participants
            .Where(p => p.User == user)
            .Where(p => p.Event == @event)
            .FirstOrDefaultAsync();
    }

    public async Task<Participant?> GetParticipantAsync(Guid token)
    {
        return await Db.Participants
            .Where(p => p.Token == token)
            .Include(p => p.Event)
            .FirstOrDefaultAsync();
    }
}
