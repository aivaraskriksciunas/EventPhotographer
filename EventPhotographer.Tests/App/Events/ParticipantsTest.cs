using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Events.Entities;
using EventPhotographer.Tests.Fakers.Events;
using EventPhotographer.Tests.Fakes.Events;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace EventPhotographer.Tests.App.Events;

public class ParticipantsTest : BaseIntegrationTest
{
    public ParticipantsTest(AppWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact] 
    public async Task JoinEvent_AsAnonymous()
    {
        // Arrage 
        var shareableLink = await CreateShareableLinkAsync();

        var request = new JoinEventRequestDto
        {
            Name = "Test participant name",
            Code = shareableLink.Code,
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/participants/join", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, await Db.Participants.CountAsync());
        var participant = await Db.Participants.SingleOrDefaultAsync();
        Assert.NotNull(participant);
        Assert.Null(participant.User);
        Assert.Equal(shareableLink.Event, participant.Event);
        Assert.Equal(shareableLink, participant.EventShareableLink);
        Assert.Equal(request.Name, participant.Name);
    }

    [Fact]
    public async Task GetEvent_AfterJoining()
    {
        // Arrage 
        var shareableLink = await CreateShareableLinkAsync();

        var request = new JoinEventRequestDto
        {
            Name = "Test participant name",
            Code = shareableLink.Code,
        };

        // Act
        await Client.PostAsJsonAsync("/api/participants/join", request);
        var joinedEvent = await Client.GetFromJsonAsync<ParticipantResponseDto>("/api/participants/current");

        // Assert
        Assert.NotNull(joinedEvent);
        Assert.Equal(shareableLink.Event.Id, joinedEvent.Event.Id);
    }

    [Fact]
    public async Task GetEvent_IfNoEventJoined()
    {
        // Arrage
        var shareableLink = await CreateShareableLinkAsync();

        // Act
        var response = await Client.GetAsync("/api/participants/current");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task JoinEvent_AsLoggedIn()
    {
        // Arrage 
        var shareableLink = await CreateShareableLinkAsync();

        var request = new JoinEventRequestDto
        {
            Name = "Test participant name",
            Code = shareableLink.Code,
        };

        // Act
        var user = await CreateUserAsync();
        var client = await GetClientWithAuthAsync(user);
        var response = await client.PostAsJsonAsync("/api/participants/join", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, await Db.Participants.CountAsync());
        var participant = await Db.Participants.SingleOrDefaultAsync();
        Assert.NotNull(participant);
        Assert.Equal(user, participant.User);
        Assert.Equal(request.Name, participant.Name);
    }

    [Fact]
    public async Task JoinEvent_WithInvalidCode()
    {
        // Arrage 
        await CreateShareableLinkAsync();
        await CreateShareableLinkAsync();

        var request = new JoinEventRequestDto
        {
            Name = "Test participant name",
            Code = "000AAA",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/participants/join", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(0, await Db.Participants.CountAsync());
    }

    [Fact]
    public async Task JoinAndLeaveEvent()
    {
        // Arrange
        var shareableLink = await CreateShareableLinkAsync();
        var request = new JoinEventRequestDto
        {
            Name = "Test participant name",
            Code = shareableLink.Code,
        };

        // Join and get event
        await Client.PostAsJsonAsync("/api/participants/join", request);
        var joinedEvent = await Client.GetFromJsonAsync<ParticipantResponseDto>("/api/participants/current");
        Assert.NotNull(joinedEvent);
        Assert.Equal(joinedEvent.Event.Name, shareableLink.Event.Name);

        // Leave and get event 
        await Client.GetAsync("/api/participants/leave");
        var response = await Client.GetAsync("/api/participants/current");
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }


    private async Task<EventShareableLink> CreateShareableLinkAsync(
        EventFaker? eventFaker = null)
    {
        var user = await CreateUserAsync();
        var faker = new EventFaker()
            .RuleFor(e => e.User, (f, e) => e.User = user)
            .RuleFor(e => e.StartDate, (f, e) => f.Date.Recent(1))
            .RuleFor(e => e.EndDate, (f, e) => f.Date.Soon());

        var @event = faker.Generate();
        var shareableLink = new EventShareableLinkFaker()
            .RuleFor(e => e.Event, (f, e) => e.Event = @event)
            .Generate();

        await Db.Events.AddAsync(@event);
        await Db.EventShareableLinks.AddAsync(shareableLink);
        await Db.SaveChangesAsync();

        return shareableLink;
    }
}
