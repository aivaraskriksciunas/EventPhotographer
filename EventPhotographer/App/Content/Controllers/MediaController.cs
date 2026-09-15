using EventPhotographer.App.Events.Services;
using EventPhotographer.App.Content.DTO;
using EventPhotographer.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using EventPhotographer.App.Content.Mappers;
using FluentValidation;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.Core.Extensions;
using EventPhotographer.UseCases.Content.Commands;
using EventPhotographer.Core.Features.Content.Entities;
using Microsoft.Extensions.Options;
using EventPhotographer.Core.Configuration;

namespace EventPhotographer.App.Content.Controllers;

public class MediaController(
    MediaService mediaService)
    : ApiController
{
    [HttpPost]
    [ActiveParticipantRequired]
    public async Task<ActionResult<CreateMediaResponseDto>> Create(
        [FromBody] MediaUploadRequestDto uploadRequest,
        [FromServices] ICommandHandler<CreateMediaCommand, CreateMediaResult> handler,
        [FromServices] IValidator<MediaUploadRequestDto> validator)
    {
        await validator.ValidateAndThrowAsync(uploadRequest);

        var participant = HttpContext.GetParticipant();
        if (participant?.Event == null)
        {
            return Forbid();
        }

        var result = await handler.HandleAsync(new CreateMediaCommand
        {
            Participant = participant,
            Event = participant.Event,
            FileType = uploadRequest.FileType!,
            FileSize = uploadRequest.FileSize
        });

        if (!result.IsSuccess)
        {
            return result.ToProblemDetailsResult();
        }

        var resultObject = result.Value;
        if (resultObject.UploadUrl == null)
        {
            resultObject.UploadUrl = Url.Action(nameof(UploadFile), "Media", new { mediaFileId = resultObject.MediaFile.Id });
        }

        return MediaMapper.ToResponse(result.Value);
    }

    [HttpPut("File/{mediaFileId:guid}/Upload")]
    [RequestSizeLimit(50_000_000)] // 50 MB
    [ActiveParticipantRequired]
    public async Task<IActionResult> UploadFile(
        Guid mediaFileId,
        IFormFile file,
        [FromServices] MediaStorageService storageService,
        [FromServices] IOptions<ObjectStorageConfiguration> options)
    {
        if (options.Value.UsePresignedUploadUrls != false)
        {
            return NotFound();
        }

        var participant = HttpContext.GetParticipant();
        var mediaFile = await mediaService.GetFileByIdAsync(mediaFileId);
        if (mediaFile == null || mediaFile.Media.ParticipantId != participant?.Id)
        {
            return NotFound();
        }

        await using var stream = file.OpenReadStream();
        await storageService.UploadFile(stream, file.ContentType, mediaFile.Path);

        return Ok();
    }

    [HttpGet("{mediaId:guid}/Status")]
    public async Task<ActionResult<MediaResponseDto>> GetMedia(
        Guid mediaId,
        [FromServices] ICommandHandler<ValidateMediaCommand, Media> handler)
    {
        var participant = HttpContext.GetParticipant();
        var result = await handler.HandleAsync(new ValidateMediaCommand
        {
            Participant = participant,
            Id = mediaId,
        });

        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return MediaMapper.ToResponse(result.Value);
    }

    [HttpGet("File/{fileId:guid}")]
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
