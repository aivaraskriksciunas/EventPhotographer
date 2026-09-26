using EventPhotographer.Core.Entities.AccountPolicies;
using EventPhotographer.Core.Features.Emails.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Features.Users.Entities;

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

    public ICollection<AccountTier> TierRecords = new List<AccountTier>();

    public ICollection<Email> SentEmails = new List<Email>();

    public ICollection<AccountVerification> AccountVerifications = new List<AccountVerification>();
}
