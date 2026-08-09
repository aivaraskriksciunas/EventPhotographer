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

namespace EventPhotographer.App.Content.Controllers;

public class MediaController (
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

        return MediaMapper.ToResponse(result.Value);
    }

    [HttpGet("{mediaId:guid}/status")]
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
