using EventPhotographer.App.Events.Authorization;
using EventPhotographer.App.Events.DTO.Request;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.App.Events.Mappers;
using EventPhotographer.App.Events.Services;
using EventPhotographer.Core.Attributes;
using EventPhotographer.Core.Extensions;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Middleware;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.App.Events.Controllers;

public class ParticipantsController(
    UserManager<User> userManager) 
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

    [HttpGet]
    [Route("Check/{code}")]
    public async Task<ActionResult<EventShareableLinkDetailResponseDto>> GetShareableLinkByCode(
        string code,
        [FromServices] EventShareableLinkService shareableLinkService)
    {
        var shareableLink = await shareableLinkService.GetShareableLinkByCode(code);
        if (shareableLink == null)
        {
            return NotFound();
        }

        return EventShareableLinkMapper.CreateDetailResponseDto(shareableLink);
    }

    [HttpPost]
    [Route("Join")]
    public async Task<ActionResult<ParticipantResponseDto>> Join(
        [FromBody] JoinEventRequestDto resource,
        [FromServices] ICommandHandler<JoinEvent, Participant> commandHandler)
    {
        var user = await userManager.GetUserAsync(User);
        var result = await commandHandler.HandleAsync(new JoinEvent
        {
            User = user,
            Code = resource.Code,
            Name = resource.Name,
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        var participant = result.Value;
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
