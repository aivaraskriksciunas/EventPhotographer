using EventPhotographer.App.Events.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPhotographer.Core;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
}
