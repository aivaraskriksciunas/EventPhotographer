using EventPhotographer.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Tests;

[Collection("SharedResourceTestCollection")]
public abstract class BaseIntegrationTest : IClassFixture<AppWebApplicationFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly AppWebApplicationFactory _factory;
    protected HttpClient Client { get; }
    protected AppDbContext Db { get; }

    public BaseIntegrationTest(AppWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.HttpClient;

        _scope = factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }
}
