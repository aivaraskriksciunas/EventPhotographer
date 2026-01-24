using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.App.Users.Entities;

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
}
