using EventPhotographer.Core;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Features.Users.Services;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Commands;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.UseCases.Users.Commands;

public record ValidateAccount
    : ICommand<User>
{
    public required string Token { get; set; }
}

internal class ValidateAccountHandler(
    AppDbContext db,
    AccountVerificationService accountVerificationService)
    : ICommandHandler<ValidateAccount, User>
{
    public async Task<Result<User>> HandleAsync(
        ValidateAccount command,
        CancellationToken cancellationToken = default)
    {
        var verification = await accountVerificationService.GetValidVerification(command.Token);
        if (verification == null)
        {
            return new Error("InvalidOrExpiredToken");
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == verification.UserId, cancellationToken);
        if (user == null)
        {
            return new Error("InvalidOrExpiredToken");
        }

        verification.ValidatedAt = DateTime.UtcNow;
        user.EmailConfirmed = true;

        await db.SaveChangesAsync(cancellationToken);

        return user;
    }
}
