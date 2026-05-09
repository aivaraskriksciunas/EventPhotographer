namespace EventPhotographer.Core.Startup;

public static class CorsSetup
{
    public static void ConfigureApplicationCors(this IServiceCollection services, string clientUrl)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                policy => {
                    policy.WithOrigins(clientUrl)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            );
        });
    }

    public static void UseApplicationCorsPolicy(this IApplicationBuilder app)
    {
        app.UseCors("CorsPolicy");
    }
}
