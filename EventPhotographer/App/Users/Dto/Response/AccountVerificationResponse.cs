namespace EventPhotographer.App.Users.Dto.Response;

public record AccountVerificationResponse
{
    public DateTime CreatedAt { get; init; }

    public DateTime NextResendDate { get; init; }
}
