using Riok.Mapperly.Abstractions;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.App.Users.Dto.Response;

namespace EventPhotographer.App.Users.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserMapper
{
    public static partial UserLoginResponseDto ToLoginResponseDto(User user);
}
