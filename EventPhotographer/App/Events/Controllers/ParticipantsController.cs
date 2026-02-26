using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Users.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class ParticipantsController(
    ParticipantService participantService,
    UserManager<User> userManager,
    IAuthorizationService authorizationService) 
    : ApiController
{
    [HttpPost]
    [Route("Join")]
    [AllowAnonymous]
    public async Task<ActionResult<ParticipantResponseDto>> Join(
        [FromBody] JoinEventRequestDto resource,
        [FromServices] IValidator<JoinEventRequestDto> validator,
        [FromServices] EventShareableLinkService shareableLinkService)
    {
        await validator.ValidateAndThrowAsync(resource);

        var shareableLink = await shareableLinkService.GetShareableLinkByCode(resource.Code);
        if (shareableLink == null)
        {
            return NotFound();
        }

        var authResult = await authorizationService.AuthorizeAsync(User, shareableLink.Event, new JoinEventRequirement());
        if (!authResult.Succeeded)
        {
            return NotFound();
        }

        var user = await userManager.GetUserAsync(User);
        var participant = await participantService.CreateOrGetParticipant(
            shareableLink,
            resource.Name,
            user
        );

        return Ok(EventMapper.CreateResponseDto(participant));
    }
}
