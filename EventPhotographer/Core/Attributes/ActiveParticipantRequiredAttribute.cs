using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.Core.Attributes;

internal class ActiveParticipantRequiredAttribute : AuthorizeAttribute
{
    public const string POLICY_NAME = "ActiveParticipantRequired";

    public ActiveParticipantRequiredAttribute()
    {
        Policy = POLICY_NAME;
    }
}
