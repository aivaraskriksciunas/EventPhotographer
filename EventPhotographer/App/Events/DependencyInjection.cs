using EventPhotographer.App.Events.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace EventPhotographer.App.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        services.AddScoped<Services.ApiEventService>();

        // Authorization
        services.AddSingleton<IAuthorizationHandler, ManageEventRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CreateShareableLinkRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, JoinEventRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, IsActiveParticipantRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UploadEventMediaRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, GenerateFileArchiveRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CreateWhatsAppMessageLinkRequirementHandler>();

        return services;
    }
}
