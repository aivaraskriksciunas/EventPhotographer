using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EventPhotographer.Core.Features.Users.Services;

public class AccountVerificationService(
    AppDbContext db)
{
    private const int VerificationCooldownMinutes = 1;

    private const int VerificationsPerDay = 5;

    private const int VerificationExpirationMinutes = 60 * 24; // 24 hours

    public async Task<DateTime> CalculateWhenNextVerificationCanBeGenerated(User user)
    {
        var now = DateTime.UtcNow;

        var previousVerification = await db.AccountVerifications
            .Where(a => a.User == user)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();

        if (previousVerification == null)
        {
            return now;
        }

        var verificationsToday = await db.AccountVerifications
            .Where(a => a.CreatedAt >= now.StartOfDay())
            .Where(a => a.CreatedAt <= now.EndOfDay())
            .Where(a => a.User == user)
            .CountAsync();

        if (verificationsToday >= VerificationsPerDay)
        {
            return now.AddDays(1).StartOfDay();
        }

        return previousVerification.CreatedAt.AddMinutes(VerificationCooldownMinutes);
    }

    public async Task<AccountVerification> CreateVerification(User user)
    {
        var verification = new AccountVerification
        {
            User = user,
            CreatedAt = DateTime.UtcNow,
            ValidatedAt = null,
            Code = GenerateCode()
        };

        await db.AccountVerifications.AddAsync(verification);
        await db.SaveChangesAsync();

        return verification;
    }

    public async Task<AccountVerification?> GetValidVerification(string code)
    {
        code = code.Trim();
        if (code.Length <= 10)
        {
            return null;
        }

        var verification = await db.AccountVerifications
            .Where(a => a.Code.ToLower() == code.ToLower())
            .FirstOrDefaultAsync();

        if (verification == null 
            || verification.CreatedAt.AddMinutes(VerificationExpirationMinutes) < DateTime.UtcNow)
        {
            return null;
        }

        return verification;
    }

    private string GenerateCode()
    {
        return RandomNumberGenerator.GetString("abcdefghijklmnopqrstuvwxyz0123456789", 64);
    }
}
