using EventPhotographer.App.MessagingIntegrations.DTO.Response;
using EventPhotographer.Core.Features.MessagingIntegrations.Entities;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.MessagingIntegrations.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class WhatsAppMessageLinkMapper
{
    public static partial WhatsAppMessageLinkResponseDto ToResponseDto(this WhatsAppMessageLink messageLink);
}
