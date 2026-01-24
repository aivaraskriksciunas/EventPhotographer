using EventPhotographer.App.Users.Entities;
using EventPhotographer.App.Users.Dto;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Users.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserMapper
{
    public static partial UserLoginResponseDto ToLoginResponseDto(User user);
}
