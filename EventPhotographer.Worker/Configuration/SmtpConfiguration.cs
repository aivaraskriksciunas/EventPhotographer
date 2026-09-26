namespace EventPhotographer.Worker.Configuration;

internal class SmtpConfiguration
{
    public required string Host { get; set; }

    public required int Port { get; set; }

    public required bool Tls { get; set; } = false;

    public required string FromEmail { get; set; }

    public required string FromName { get; set; }

    public required string? Username { get; set; } = null;

    public required string? Password { get; set; } = null;
}
