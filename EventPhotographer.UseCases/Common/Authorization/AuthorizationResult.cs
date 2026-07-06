namespace EventPhotographer.UseCases.Common.Authorization;

public class AuthorizationResult
{
    public bool IsAuthorized { get; init; }

    public static AuthorizationResult Success() => new() { IsAuthorized = true };
    public static AuthorizationResult Failure() => new() { IsAuthorized = false };
}
