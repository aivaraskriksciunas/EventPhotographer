using EventPhotographer.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace EventPhotographer.Tests;

public sealed class AppWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:alpine")
        .WithDatabase("eventphotographer_db")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    private DbConnection _dbConnection = null!;

    private Respawner _respawner = null!;

    public HttpClient HttpClient { get; private set; } = null!; 

    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        _dbConnection = new NpgsqlConnection(_db.GetConnectionString());
        HttpClient = CreateClient();

        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            
            services.AddDbContext<AppDbContext>((container, options) =>
            {
                options.UseNpgsql(_dbConnection);
            });

            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthenticationHandler.SCHEME;
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SCHEME;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SCHEME;
            });

            services.RemoveAll<AuthenticationService>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = TestAuthenticationHandler.SCHEME;
            })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SCHEME, options => { });
        });

        builder.UseEnvironment("Development");
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _db.DisposeAsync();
        await _dbConnection.DisposeAsync();
    }
}
