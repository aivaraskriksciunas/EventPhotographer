using EventPhotographer.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core.Startup;

public class DatabaseStartup : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseStartup> _logger;

    public DatabaseStartup(
        IServiceProvider services,
        ILogger<DatabaseStartup> logger) 
    { 
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running database migrations...");
        using (var scope = _services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
