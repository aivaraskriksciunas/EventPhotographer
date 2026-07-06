namespace EventPhotographer.UseCases.Common;

using EventPhotographer.UseCases.Common.Authorization;
using FluentValidation.Results;

public enum ErrorType
{
    None,
    BusinessLogicError,
    NotFound,
    AccessDenied,
    ValidationFailed,
}

public record Error(string Code, ErrorType ErrorType = ErrorType.BusinessLogicError)
{
    public static readonly Error None = new(string.Empty, ErrorType.None);

    public static readonly Error NotFound = new("NotFound", ErrorType.NotFound);

    public static readonly Error AccessDenied = new("AccessDenied", ErrorType.AccessDenied);
}

public record ValidationError(ValidationResult ValidationResult)
    : Error("ValidationFailed", ErrorType.ValidationFailed)
{}

public record AccessDeniedError(AuthorizationResult AuthorizationResult)
    : Error("AccessDenied", ErrorType.AccessDenied)
{ }