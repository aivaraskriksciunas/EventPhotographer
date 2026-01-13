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
    public async Task Should_CreateEvent()
    {
        // Arrange 
        var request = new
        {
            Name = "Test event"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/Events", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, await Db.Events.CountAsync());
    }

    [Fact]
    public async Task Should_GetEvent()
    {
        // Arrange 
        var entity = new EventPhotographer.App.Events.Entities.Event
        {
            Name = "Test event",
            CreatedAt = DateTime.UtcNow,
        };
        await Db.Events.AddAsync(entity);
        await Db.SaveChangesAsync();

        var events = await Db.Events.ToListAsync();
        Assert.Equal(1, await Db.Events.CountAsync());

        // Act
        var response = await Client.GetAsync($"/api/Events/{entity.Id}");

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returnedEvent = await response.Content.ReadFromJsonAsync<EventPhotographer.App.Events.Entities.Event>();
        Assert.NotNull(returnedEvent);
        Assert.Equal(entity.Name, returnedEvent.Name);
        Assert.Equal(entity.CreatedAt.ToShortDateString(), entity.CreatedAt.ToShortDateString());
        Assert.Equal(entity.Id, returnedEvent.Id);
    }

    [Fact]
    public async Task Should_Return404()
    {
        var response = await Client.GetAsync($"/api/Events/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
