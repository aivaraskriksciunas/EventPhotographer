using EventPhotographer.UseCases.Common;
using Microsoft.AspNetCore.Mvc;

namespace EventPhotographer.Core.Extensions;

public static class ResultExtensions
{
    public static ObjectResult ToProblemDetailsResult(this Result result, bool accessDeniedAsNotFound = true)
    {
        if (result.Error is AccessDeniedError && accessDeniedAsNotFound)
        {
            // Meant to hide the existence of a resource from unauthorized users
            result = Result.Failure(Error.NotFound);
        }

        var problemDetails = result.Error.ErrorType switch
        {
            ErrorType.AccessDenied => new ProblemDetails
            {
                Detail = result.Error.Code,
                Status = StatusCodes.Status403Forbidden
            },
            ErrorType.NotFound => new ProblemDetails
            {
                Detail = result.Error.Code,
                Status = StatusCodes.Status404NotFound
            },
            ErrorType.ValidationFailed => new ValidationProblemDetails
            {
                Detail = result.Error.Code,
                Status = StatusCodes.Status400BadRequest
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest
            }
        };

        if (result.Error is ValidationError validationError)
        {
            var errors = validationError.ValidationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            var validationProblemDetails = (ValidationProblemDetails)problemDetails;
            validationProblemDetails.Errors = errors;

            new ObjectResult(problemDetails);
        }

        return new ObjectResult(problemDetails);
    }
}
