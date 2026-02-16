using EventPhotographer.App.Events.Resources;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace EventPhotographer.Tests.App.Events;

public class EventsTests : BaseIntegrationTest
{
    public EventsTests(AppWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateEvent_WithValidData()
    {
        // Arrange 
        var user = await CreateUserAsync();
        var request = new EventDto
        {
            Name = "Test event",
            StartDate = DateTime.Now,
            EventDuration = EventDuration.OneDay.ToString(),
        };

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.PostAsJsonAsync("/api/Events", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, await Db.Events.CountAsync());
    }

    public static IEnumerable<object[]> GetInvalidCreateEventData()
    {
        var list = new List<object[]>
        {
            new object[] { new { Name = "", StartDate = DateTime.UtcNow, EventDuration = EventDuration.OneDay } },
            new object[] { new { Name = "a", StartDate = DateTime.UtcNow, EventDuration = EventDuration.OneDay } },
            new object[] { new { Name = "correct name", StartDate = DateTime.UtcNow, EventDuration = "Hello" } },
            new object[] { new { Name = "correct name", StartDate = DateTime.UtcNow.AddDays(-1), EventDuration = EventDuration.OneDay } },
            new object[] { new { Name = "correct name", StartDate = DateTime.UtcNow } }
        };

        return list;
    }

    [Theory]
    [MemberData(nameof(GetInvalidCreateEventData))]
    public async Task CreateEvent_WithInvalidData(object request)
    {
        // Act
        var response = await Client.PostAsJsonAsync("/api/Events", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await Db.Events.CountAsync());
    }

    [Fact]
    public async Task GetEvent_ExistingEvent()
    {
        // Arrange 
        var user = await CreateUserAsync();
        var entity = new EventPhotographer.App.Events.Entities.Event
        {
            Name = "Test event",
            CreatedAt = DateTime.UtcNow,
            User = user,
        };
        await Db.Events.AddAsync(entity);
        await Db.SaveChangesAsync();

        Assert.Equal(1, await Db.Events.CountAsync());

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{entity.Id}");

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returnedEvent = await response.Content.ReadFromJsonAsync<EventPhotographer.App.Events.Entities.Event>();
        Assert.NotNull(returnedEvent);
        Assert.Equal(entity.Name, returnedEvent.Name);
        Assert.Equal(entity.CreatedAt.ToShortDateString(), entity.CreatedAt.ToShortDateString());
        Assert.Equal(entity.Id, returnedEvent.Id);
    }

    [Fact]
    public async Task GetEvent_EnsurePermissions()
    {
        // Arrange 
        var entity = new EventPhotographer.App.Events.Entities.Event
        {
            Name = "Test event",
            CreatedAt = DateTime.UtcNow,
            User = await CreateUserAsync(),
        };
        await Db.Events.AddAsync(entity);
        await Db.SaveChangesAsync();

        Assert.Equal(1, await Db.Events.CountAsync());

        // Act
        var client = await GetClientWithAuthAsync();
        var response = await client.GetAsync($"/api/Events/{entity.Id}");

        // Assert 
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEvent_WithValidData()
    {
        // Arrange 
        var user = await CreateUserAsync();
        var entity = new EventPhotographer.App.Events.Entities.Event
        {
            Name = "Test event",
            User = user,
            CreatedAt = DateTime.UtcNow,
        };
        await Db.Events.AddAsync(entity);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var updateRequest = new
        {
            Name = "Updated event",
            EventDuration = EventDuration.OneDay.ToString(),
        };
        var response = await client.PutAsJsonAsync($"/api/Events/{entity.Id}", updateRequest);

        // Assert
        await Db.Entry(entity).ReloadAsync();
        var updatedEvent = await Db.Events.FindAsync(entity.Id);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedEvent);
        Assert.Equal("Updated event", updatedEvent.Name);
    }

    [Fact]
    public async Task UpdateEvent_EnsurePermissions()
    {
        // Arrange 
        var entity = new EventPhotographer.App.Events.Entities.Event
        {
            Name = "Test event",
            User = await CreateUserAsync(),
            CreatedAt = DateTime.UtcNow,
        };
        await Db.Events.AddAsync(entity);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync();
        var updateRequest = new
        {
            Name = "Updated event",
            EventDuration = EventDuration.OneDay.ToString(),
        };
        var response = await client.PutAsJsonAsync($"/api/Events/{entity.Id}", updateRequest);

        // Assert
        await Db.Entry(entity).ReloadAsync();
        var updatedEvent = await Db.Events.FindAsync(entity.Id);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(updatedEvent);
        Assert.Equal("Test event", updatedEvent.Name);
    }

    [Fact]
    public async Task GetEvent_InexistantEvent()
    {
        var response = await Client.GetAsync($"/api/Events/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
