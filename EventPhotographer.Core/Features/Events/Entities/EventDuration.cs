using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace EventPhotographer.Core.Features.Events.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventDuration
{
    OneHour,
    TwoHours,
    ThreeHours,
    SixHours,
    TwelveHours,
    OneDay,
    TwoDays,
    FiveDays,
    OneWeek,
}
