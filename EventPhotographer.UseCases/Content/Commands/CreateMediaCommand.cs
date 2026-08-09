using Amazon.S3;
using EasyNetQ;
using EventPhotographer.Core;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Core.Features.Content.Services;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Util;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Events.Authorization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.UseCases.Content.Commands;

public record CreateMediaCommand : 
    ICommand<CreateMediaResult>,
    IRequiresResourceAuthorization
{
    public required Participant Participant;

    public required Event Event;

    public required string FileType;

    public required long FileSize;

    public IEntity GetAuthorizationResource() => Event;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new UploadEventMediaRequirement();
    }
}

public record CreateMediaResult
{
    public required Media Media;

    public required string UploadUrl;
}

internal class CreateMediaValidator : AbstractValidator<CreateMediaCommand>
{
    public CreateMediaValidator()
    {
        RuleFor(x => x.Participant).NotNull();
        RuleFor(x => x.Event).NotNull();
        RuleFor(x => x.FileType)
            .NotEmpty()
            .Must((value) => FileContentTypeReader.IsAllowedMimeType(value!))
            .WithMessage("File type is not allowed.");
        RuleFor(x => x.FileSize)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("File size is required and must be a positive number.")
            .LessThan(50_000_000)
            .WithMessage("File size may not exceed 50MB");
    }
}

internal class CreateMediaHandler(
    AppDbContext db,
    MediaStorageService mediaStorageService,
    IBus bus) : 
    ICommandHandler<CreateMediaCommand, CreateMediaResult>
{
    public async Task<Result<CreateMediaResult>> HandleAsync(CreateMediaCommand command, CancellationToken cancellationToken = default)
    {
        var fileTypeInfo = FileContentTypeReader.GetFileTypeFromMimeType(command.FileType);
        if (fileTypeInfo is null)
        {
            return new Error("FileTypeNotAllowed");
        }

        var key = Guid.NewGuid().ToString() + fileTypeInfo.Extension;
        var uploadUrl = await mediaStorageService.CreatePresignedUrl(
            key,
            command.FileSize);

        var media = new Media
        {
            Event = command.Event,
            Participant = command.Participant,
            Status = MediaStatus.NeedsValidation,
            Type = MediaType.UserUpload,
        };

        var mediaFile = new MediaFile
        {
            Media = media,
            Path = key,
            MimeType = command.FileType,
            FileSize = (ulong)command.FileSize,
        };

        await db.AddAsync(media, cancellationToken);
        await db.AddAsync(mediaFile, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await bus.PubSub.PublishAsync(new ValidateUploadedFileMessage { MediaFileId = mediaFile.Id, DelayValidationMs = 5000 });

        return new CreateMediaResult
        {
            Media = media,
            UploadUrl = uploadUrl,
        };
    }
}