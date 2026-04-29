using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Data;

public static class DependencyInjection
{
    public static void AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(connectionString)
        );
    }
}
