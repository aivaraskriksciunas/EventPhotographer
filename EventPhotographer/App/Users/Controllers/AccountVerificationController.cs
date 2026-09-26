using EventPhotographer.App.Users.Dto.Response;
using EventPhotographer.Core.Extensions;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Users.Controllers;

[Route("api/Account/Verification")]
public class AccountVerificationController(
    UserManager<User> userManager)
    : ApiController
{
    [HttpGet]
    [Route("Send")]
    [Authorize]
    public async Task<ActionResult<AccountVerificationResponse>> StartVerification(
        [FromServices] ICommandHandler<StartAccountVerification, StartAccountVerificationResult> commandHandler)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await commandHandler.HandleAsync(new StartAccountVerification
        {
            User = user,
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok(new AccountVerificationResponse
        {
            CreatedAt = result.Value.AccountVerification.CreatedAt,
            NextResendDate = result.Value.NextResendDate,
        });
    }

    [HttpGet]
    [Route("Verify")]
    public async Task<ActionResult> Verify(
        [FromQuery] string token,
        [FromServices] ICommandHandler<ValidateAccount, User> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new ValidateAccount
        {
            Token = token,
        });
        
        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        return Ok();
    }
}
