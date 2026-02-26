using EventPhotographer.App.Events.Entities;
using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace EventPhotographer.App.Events.Services;

public class EventShareableLinkService(
    AppDbContext Db,
    IOptions<ShareableLinkOptions> options)
{

    private readonly ShareableLinkOptions linkOptions = options.Value;

    public async Task<IEnumerable<EventShareableLink>> GetShareableLinks(Event @event)
    {
        return await Db.EventShareableLinks
            .Where(x => x.Event!.Id == @event.Id)
            .ToListAsync();
    }

    public async Task<EventShareableLink?> GetShareableLinkByCode(string code)
    {
        return await Db.EventShareableLinks
            .Where(x => x.Code == code)
            .Include(x => x.Event)
            .FirstOrDefaultAsync()
        ;
    }

    public async Task<EventShareableLink> CreateShareableLink(Event @event)
    {
        var entity = new EventShareableLink
        {
            Event = @event,
            Code = await GenerateUniqueCode()
        };

        await Db.EventShareableLinks.AddAsync(entity);
        await Db.SaveChangesAsync();

        return entity;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var code = string.Empty;
        do
        {
            code = RandomNumberGenerator.GetString(linkOptions.AllowedCodeChars, 6);
        } while (await Db.EventShareableLinks.AnyAsync(x => x.Code == code));

        return code;
    }
}
