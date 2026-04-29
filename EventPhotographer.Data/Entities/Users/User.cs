using EventPhotographer.Data.Entities.AccountPolicies;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Data.Entities.Users;

public class User : IdentityUser
{
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public override string? Email
    {
        get => base.Email;
        set
        {
            base.Email = value;
            UserName = value;
        }
    }

    public ICollection<AccountTier> TierRecords = new Collection<AccountTier>();
}
