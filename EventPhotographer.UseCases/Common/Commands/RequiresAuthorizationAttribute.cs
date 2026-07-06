using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Common.Commands;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal class RequiresAuthorizationAttribute : Attribute
{
    public readonly IAuthorizationRequirement Requirement;

    public RequiresAuthorizationAttribute(IAuthorizationRequirement requirement)
    {
        Requirement = requirement;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
internal class RequiresResourceAuthorizationAttribute : Attribute
{
    public readonly IAuthorizationRequirement Requirement;
    public RequiresResourceAuthorizationAttribute(IAuthorizationRequirement requirement)
    {
        Requirement = requirement;
    }
}