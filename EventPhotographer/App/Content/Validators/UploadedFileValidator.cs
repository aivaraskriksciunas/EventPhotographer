using EventPhotographer.App.Content.Services;
using EventPhotographer.Core.Features.Content.Services;
using FluentValidation;

namespace EventPhotographer.App.Content.Validators;

public class UploadedFileValidator : AbstractValidator<IFormFile>
{
    private FileContentTypeReader _contentTypeReader;

    private const long MaxFileSizeInBytes = 50 * 1024 * 1024; // 50 MB

    public UploadedFileValidator(
        FileContentTypeReader contentTypeReader)
    {
        _contentTypeReader = contentTypeReader;

        RuleFor(file => file)
            .Must(ValidateContentType)
            .Must(file => file.Length <= MaxFileSizeInBytes);
    }


    private bool ValidateContentType(IFormFile file)
    {
        bool isValid = _contentTypeReader.DetermineFileExtension(file.OpenReadStream()) != null;

        return isValid;
    }
}
