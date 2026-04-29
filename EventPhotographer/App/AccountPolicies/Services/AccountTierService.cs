using EventPhotographer.Data.Entities.AccountPolicies;
using EventPhotographer.Data.Entities.Users;
using EventPhotographer.Data;
using Microsoft.EntityFrameworkCore;

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
