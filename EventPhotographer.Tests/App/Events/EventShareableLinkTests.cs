using Bogus;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Users.Entities;
using EventPhotographer.Tests.Fakes.Events;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace EventPhotographer.Tests.App.Events;

public class EventShareableLinkTests : BaseIntegrationTest
{
    public EventShareableLinkTests(AppWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateShareableLink_WithValidDataOnlyOnce()
    {
        // Arrange 
        var @event = await CreateEvent();

        // Act
        var client = await GetClientWithAuthAsync(@event.User);
        var response = await client.PostAsync($"/api/Events/{@event.Id}/ShareableLinks", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await response.Content.ReadFromJsonAsync<EventShareableLinkResponseDto>();
        Assert.NotNull(data);
        Assert.Equal(1, await Db.EventShareableLinks.CountAsync());
        var link = await Db.EventShareableLinks.FirstOrDefaultAsync();
        Assert.Equal(@event, link!.Event);

        // Send the same request again
        response = await client.PostAsync($"/api/Events/{@event.Id}/ShareableLinks", null);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(1, await Db.EventShareableLinks.CountAsync());
    }

    [Fact]
    public async Task CreateShareableLink_AsDifferentUser()
    {
        // Arrange 
        var @event = await CreateEvent();
        var anotherUser = await CreateUserAsync();

        // Act
        var client = await GetClientWithAuthAsync(anotherUser);
        var response = await client.PostAsync($"/api/Events/{@event.Id}/ShareableLinks", null);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, await Db.EventShareableLinks.CountAsync());
    }

    [Fact]
    public async Task CreateShareableLink_ForPassedEvent()
    {
        // Arrange 
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) =>
            {
                e.User = user;
                e.EndDate = f.Date.Past();
            })
            .Generate();

        await Db.AddAsync(@event);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.PostAsync($"/api/Events/{@event.Id}/ShareableLinks", null);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, await Db.EventShareableLinks.CountAsync());
    }

    private async Task<Event> CreateEvent()
    {
        var user = await CreateUserAsync();
        var faker = new EventFaker()
            .Rules((f, e) =>
            {
                e.User = user;
            });
        
        var @event = faker.Generate();
        await Db.AddAsync(@event);
        await Db.SaveChangesAsync();

        return @event;
    }
}
