namespace EventPhotographer.Core.Startup;

public static class CorsSetup
{
    public static void ConfigureApplicationCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy",
                policy => {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowCredentials()
                        .WithHeaders("content-type")
                        .AllowAnyMethod();
                }
            );
        });
    }

    public static void UseDevelopmentCorsPolicy(this IApplicationBuilder app)
    {
        app.UseCors("DevelopmentPolicy");
    }
}
