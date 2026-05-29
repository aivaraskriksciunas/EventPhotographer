using EventPhotographer.App.Events.Authorization;
using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Content.DTO;
using EventPhotographer.App.Content.Services;
using EventPhotographer.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.App.Content.Mappers;
using EventPhotographer.App.Content.Authorization;
using FluentValidation;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Users.Entities;

namespace EventPhotographer.App.Content.Controllers;

public class MediaController (
    ApiMediaService mediaService,
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
    [RequestSizeLimit(50_000_000)] // 50 MB
    [ActiveParticipantRequired]
    public async Task<IActionResult> UploadFile(
        Guid uploadToken,
        IFormFile file,
        [FromServices] IValidator<IFormFile> fileValidator, 
        [FromServices] ParticipantService participantService,
        [FromServices] UserManager<User> userManager,
        [FromServices] MediaStorageService storageService,
        [FromServices] FileContentTypeReader fileContentTypeReader)
    {
        await fileValidator.ValidateAndThrowAsync(file);

        var participant = HttpContext.GetParticipant();
        var media = await mediaService.GetByUploadTokenAsync(uploadToken);
        if (media == null)
        {
            return NotFound();
        }

        var result = await authorizationService.AuthorizeAsync(
            User,
            media,
            [new ManageMediaRequirement(), new UploadFileRequirement()]);

        if (!result.Succeeded)
        {
            return NotFound();
        }

        using var readStream = file.OpenReadStream();
        await mediaService.UploadFile(
            media, 
            readStream, 
            fileContentTypeReader.DetermineFileExtension(readStream)!);

        return Ok();
    }

    [HttpGet("file/{fileId:guid}")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFile(
        Guid fileId,
        [FromServices] MediaStorageService mediaStorageService)
    {
        var file = await mediaService.GetFileByIdAsync(fileId);
        if (file == null)
        {
            return NotFound(); 
        }

        var fileStream = await mediaStorageService.GetFileAsync(file.Path);
        if (fileStream == null)
        {
            return NotFound();
        }

        return new FileStreamResult(fileStream.ResponseStream, file.MimeType)
        {
            FileDownloadName = file.Path
        };
    }
}
