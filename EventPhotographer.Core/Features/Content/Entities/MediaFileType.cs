using System.Text.Json.Serialization;

namespace EventPhotographer.Core.Features.Content.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MediaFileType
{
    Original,
    Thumbnail,
}
