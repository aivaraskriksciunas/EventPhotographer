using EventPhotographer.Core;
using Microsoft.EntityFrameworkCore;
using EventPhotographer.Core.Entities.AccountPolicies;
using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.App.AccountPolicies.Services;

public class AccountTierService(AppDbContext Db)
{
    
    public async Task<AccountTierType> GetCurrentTierAsync(User user)
    {
        var tier = await Db.Entry(user)
            .Collection(u => u.TierRecords)
            .Query()
            .Where(t => t.From <= DateTime.UtcNow)
            .Where(t => t.To <= DateTime.UtcNow || t.To == null)
            .FirstOrDefaultAsync();

        return tier?.Type ?? AccountTierType.FreeTier;
    }
}
