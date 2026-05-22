using EventPhotographer.App.Events.Authorization;
using EventPhotographer.App.Events.DTO.Request;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Attributes;
using EventPhotographer.Core.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.App.Events.Controllers;

public class ParticipantsController(
    ParticipantService participantService,
    UserManager<User> userManager,
    IAuthorizationService authorizationService) 
    : ApiController
{
    [HttpGet]
    [Route("Current")]
    [ActiveParticipantRequired]
    public async Task<ActionResult<ParticipantResponseDto>> Get()
    {
        var participant = Request.HttpContext.GetParticipant();

        return Ok(EventMapper.CreateResponseDto(participant!));
    }

    [HttpGet]
    [Route("Leave")]
    public ActionResult Leave()
    {
        Response.Cookies.Delete(ParticipantMiddleware.HTTP_COOKIE_NAME);

        return Ok();
    }

    [HttpPost]
    [Route("Join")]
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

        Response.Cookies.Append(
            ParticipantMiddleware.HTTP_COOKIE_NAME,
            participant.Token.ToString(),
            new CookieOptions
            {
                Expires = participant.Event.EndDate.AddDays(1),
                HttpOnly = true,
                IsEssential = true
            });

        return Ok(EventMapper.CreateResponseDto(participant));
    }
}
