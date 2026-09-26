using System.Text.Json.Serialization;

namespace EventPhotographer.Core.Features.Emails.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailType
{
    EMAIL_VERIFICATION = 0,
}
