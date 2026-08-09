using EasyNetQ;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Commands;
using FluentValidation;

namespace EventPhotographer.UseCases.Content.Commands;

public record UploadFileCommand : ICommand<MediaFile>
{
    public required Event Event;
    public Participant? Participant;
    public MediaType MediaType = MediaType.UserUpload;
    public required Stream Stream;
    public required FileContentTypeInfo FileContentTypeInfo;
}

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.Event).NotNull();
        RuleFor(x => x.Participant)
            .Must((command, participant)
                => participant == null || participant.EventId == command.Event.Id);
    }
}

internal class UploadFileHandler(
    AppDbContext dbContext,
    MediaStorageService mediaStorageService,
    IBus bus)
    : ICommandHandler<UploadFileCommand, MediaFile>
{
    public async Task<Result<MediaFile>> HandleAsync(UploadFileCommand command, CancellationToken cancellationToken = default)
    {
        var media = new Media
        {
            Event = command.Event,
            Participant = command.Participant,
            UploadToken = Guid.NewGuid(),
            Type = command.MediaType,
        };

        if (command.MediaType == MediaType.UserUpload)
        {
            media.Status = MediaStatus.NeedsValidation;
        }

        // Upload file to storage
        var fileLength = (ulong)command.Stream.Length;
        var path = await mediaStorageService.UploadFile(
            command.Stream,
            command.FileContentTypeInfo.MimeType,
            Guid.NewGuid().ToString() + command.FileContentTypeInfo.Extension
        );

        var mediaFile = new MediaFile
        {
            Media = media,
            MimeType = command.FileContentTypeInfo.MimeType,
            Path = path,
            FileSize = fileLength,
        };

        await dbContext.AddAsync(media);
        await dbContext.AddAsync(mediaFile);
        await dbContext.SaveChangesAsync();

        if (media.Type == MediaType.UserUpload)
        {
            await bus.PubSub.PublishAsync(new ValidateUploadedFileMessage { MediaFileId = mediaFile.Id });
        }

        return mediaFile;
    }
}