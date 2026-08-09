using EventPhotographer.Core.Features.Content.Services;
using FluentValidation;

namespace EventPhotographer.App.Content.DTO;

public class MediaUploadRequestDto
{
    public string? FileType { get; set; }

    public long FileSize { get; set; }
}

internal class MediaUploadRequestDtoValidator : AbstractValidator<MediaUploadRequestDto>
{ 
    public MediaUploadRequestDtoValidator()
    {
        RuleFor(x => x.FileType)
            .NotEmpty()
            .WithMessage("File type is required.");

        RuleFor(x => x.FileSize)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("File size is required and must be a positive number.");
    }
}
