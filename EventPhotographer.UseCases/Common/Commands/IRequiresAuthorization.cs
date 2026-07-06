using EventPhotographer.Core.Util;
using EventPhotographer.UseCases.Common.Authorization;

namespace EventPhotographer.UseCases.Common.Commands;

internal interface IRequiresAuthorization
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements();
}

internal interface IRequiresResourceAuthorization : IRequiresAuthorization
{
    public IEntity GetAuthorizationResource();
}
