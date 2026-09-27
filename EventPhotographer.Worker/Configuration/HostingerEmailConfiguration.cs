using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Worker.Configuration;

internal class HostingerEmailConfiguration
{
    [Required]
    public string? Token { get; set; } = null;

    [Required]
    public string? MailboxResourceId { get; set; } = null;

    public string SenderDisplayName { get; set; } = "LiveAlbum.eu";
}
