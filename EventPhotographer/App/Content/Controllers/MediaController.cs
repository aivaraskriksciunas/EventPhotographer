using EventPhotographer.App.Events.Authorization.Requirements;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Content.DTO;
using EventPhotographer.App.Content.Services;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using EventPhotographer.App.Content.Mappers;
using EventPhotographer.App.Content.Authorization;
using Amazon.S3;

namespace EventPhotographer.App.Content.Controllers;

public class MediaController (
    MediaService mediaService,
    IAuthorizationService authorizationService) : ApiController
{
    [HttpPost]
    [ActiveParticipantRequired]
    public async Task<ActionResult<MediaResponseDto>> Create(
        [FromBody] MediaUploadRequestDto uploadRequest,
        [FromServices] ParticipantService participantService)
    {
        var participant = HttpContext.GetParticipant();
        if (participant?.Event == null)
        {
            return Forbid();
        }

        var authResult = await authorizationService.AuthorizeAsync(User, participant.Event, new UploadEventMediaRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var media = await mediaService.CreateMedia(uploadRequest, participant);

        return MediaMapper.ToResponse(media);
    }

    [HttpPost("{uploadToken:guid}/Upload")]
    [ActiveParticipantRequired]
    public async Task<IActionResult> UploadFile(
        Guid uploadToken,
        IFormFile file,
        [FromServices] ParticipantService participantService,
        [FromServices] UserManager<User> userManager,
        [FromServices] IAmazonS3 s3Client)
    {
        var participant = HttpContext.GetParticipant();
        var media = await mediaService.GetByUploadTokenAsync(uploadToken);
        if (media == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(
            User,
            media,
            new ManageMediaRequirement());

        if (!result.Succeeded)
        {
            return NotFound();
        }

        using (var fileStream = file.OpenReadStream())
        {
            await s3Client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = "event-photographer",
                Key = $"{media.Id}",
                InputStream = fileStream,
                ContentType = file.ContentType,
            });
        }

        return Ok();
    }
}
