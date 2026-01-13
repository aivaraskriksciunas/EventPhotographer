using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Startup;

public static class DatabaseStartup
{

    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
