using Bogus;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
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
    protected UserManager<User> UserManager { get; }

    public BaseIntegrationTest(AppWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true,
        });

        _scope = factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task<HttpClient> GetClientWithAuthAsync(User? user = null)
    {
        if (user == null)
        {
            user = new User
            {
                Email = "test@test.com",
                UserName = "test@test.com",
                Name = "Test User",
            };
        }

        await EnsureUserRegisteredAsync(user);

        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true,
        });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: TestAuthenticationHandler.SCHEME);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.USER_EMAIL_HEADER, user.Email);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.USER_ID_HEADER, user.Id);

        return client;
    }

    protected async Task<User> CreateUserAsync(string? email = null, string password = "Secret!123")
    {
        var faker = new Faker();
        email = email ?? faker.Person.Email;
        var user = new User
        {
            Email = email,
            UserName = email,
            Name = "Test User",
        };

        return await EnsureUserRegisteredAsync(user, password);
    }

    private async Task<User> EnsureUserRegisteredAsync(User user, string password = "Secret!123")
    {
        if (Db.Entry(user).State == EntityState.Detached)
        {
            var identityresult = await UserManager.CreateAsync(user, password);

            if (!identityresult.Succeeded)
            {
                throw new IntegrationTestingException();
            }
        }

        return user;
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
