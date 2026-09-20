using Bogus;
using EventPhotographer.Core.Features.Content.Entities;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Tests.Fakes.Events;
using EventPhotographer.UseCases.Common.Queries;
using EventPhotographer.UseCases.Content.Queries;
using System.Net;
using System.Net.Http.Json;

namespace EventPhotographer.Tests.App.Events;

public class EventMediaTests : BaseIntegrationTest
{
    public EventMediaTests(AppWebApplicationFactory factory) : base(factory)
    {
    }

    // ========== Parameterized Tests for Both Endpoints ==========

    [Theory]
    [InlineData("")]
    [InlineData("/Archives")]
    public async Task Endpoint_EventDoesNotExist_ReturnsNotFound(string endpointSuffix)
    {
        // Arrange
        var user = await CreateUserAsync();
        var client = await GetClientWithAuthAsync(user);
        var nonExistentEventId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/Events/{nonExistentEventId}/Media{endpointSuffix}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/Archives")]
    public async Task Endpoint_UserNotAuthenticated_ActsAsDefaultUser_ReturnsNotFound(string endpointSuffix)
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        await Db.SaveChangesAsync();

        // Act - Use client without explicit auth (uses default test user id="1")
        // This user doesn't own the event, so returns NotFound
        var response = await Client.GetAsync($"/api/Events/{@event.Id}/Media{endpointSuffix}");

        // Assert - Default test user (id=1) is not the event owner
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/Archives")]
    public async Task Endpoint_UserNotEventOwner_ReturnsNotFound(string endpointSuffix)
    {
        // Arrange
        var eventOwner = await CreateUserAsync();
        var otherUser = await CreateUserAsync();
        
        var @event = new EventFaker()
            .Rules((f, e) => e.User = eventOwner)
            .Generate();
        await Db.Events.AddAsync(@event);
        await Db.SaveChangesAsync();

        // Act - Authenticate as otherUser, not the owner
        var client = await GetClientWithAuthAsync(otherUser);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media{endpointSuffix}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ========== ListMedia Specific Tests ==========

    [Fact]
    public async Task ListMedia_EventOwnerWithNoMedia_ReturnsEmptyList()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paged = await response.Content.ReadFromJsonAsync<PagedResult<MediaModel>>();
        Assert.NotNull(paged);
        Assert.Empty(paged.Items);
        Assert.Equal(0, paged.TotalCount);
        Assert.Equal(1, paged.Page);
    }

    [Fact]
    public async Task ListMedia_EventOwnerWithMedia_ReturnsMediaList()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);

        var media = CreateMediaWithFile(@event);
        await Db.Media.AddAsync(media);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paged = await response.Content.ReadFromJsonAsync<PagedResult<MediaModel>>();
        Assert.NotNull(paged);
        Assert.Single(paged.Items);
        Assert.Equal(media.Id, paged.Items[0].Id);
    }

    [Fact]
    public async Task ListMedia_MediaWithInvalidStatus_NotIncludedInResults()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        
        var mediaValid = CreateMediaWithFile(@event, status: MediaStatus.Validated);
        var mediaInvalid = CreateMediaWithFile(@event, status: MediaStatus.Invalid);
        var mediaNullStatus = CreateMediaWithFile(@event, status: null);
        
        await Db.Media.AddRangeAsync(mediaValid, mediaInvalid, mediaNullStatus);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media");

        // Assert - Only Validated and null status should be returned
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paged = await response.Content.ReadFromJsonAsync<PagedResult<MediaModel>>();
        Assert.NotNull(paged);
        Assert.Equal(2, paged.Items.Count);
        Assert.Equal(2, paged.TotalCount);
    }

    [Fact]
    public async Task ListMedia_ArchiveMedia_NotIncludedInRegularList()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        
        var userUploadMedia = CreateMediaWithFile(@event, type: MediaType.UserUpload);
        var archiveMedia = CreateMediaWithFile(@event, type: MediaType.Archive);
        
        await Db.Media.AddRangeAsync(userUploadMedia, archiveMedia);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media");

        // Assert - Only UserUpload should be returned
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paged = await response.Content.ReadFromJsonAsync<PagedResult<MediaModel>>();
        Assert.NotNull(paged);
        Assert.Single(paged.Items);
        Assert.Equal(userUploadMedia.Id, paged.Items[0].Id);
    }

    // ========== ListArchives Specific Tests ==========

    [Fact]
    public async Task ListArchives_EventOwnerWithNoArchiveMedia_ReturnsNull()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media/Archives");

        // Assert - Ok(null) returns NoContent (204) in ASP.NET Core
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ListArchives_EventOwnerWithArchiveMedia_ReturnsArchive()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        
        var archiveMedia = CreateMediaWithFile(@event, type: MediaType.Archive);
        await Db.Media.AddAsync(archiveMedia);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media/Archives");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var archive = await response.Content.ReadFromJsonAsync<MediaModel?>();
        Assert.NotNull(archive);
        Assert.Equal(archiveMedia.Id, archive.Id);
    }

    [Fact]
    public async Task ListArchives_MultipleArchives_ReturnsMostRecent()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        
        var olderArchive = CreateMediaWithFile(@event, type: MediaType.Archive, createdAt: DateTime.UtcNow.AddDays(-1));
        var newerArchive = CreateMediaWithFile(@event, type: MediaType.Archive, createdAt: DateTime.UtcNow);
        
        await Db.Media.AddRangeAsync(olderArchive, newerArchive);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media/Archives");

        // Assert - Should return the most recent archive
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var archive = await response.Content.ReadFromJsonAsync<MediaModel?>();
        Assert.NotNull(archive);
        Assert.Equal(newerArchive.Id, archive.Id);
    }

    [Fact]
    public async Task ListArchives_MediaWithoutFiles_NotIncluded()
    {
        // Arrange
        var user = await CreateUserAsync();
        var @event = new EventFaker()
            .Rules((f, e) => e.User = user)
            .Generate();
        await Db.Events.AddAsync(@event);
        
        var archiveWithoutFiles = new Media
        {
            Event = @event,
            EventId = @event.Id,
            Type = MediaType.Archive,
            Status = MediaStatus.Validated,
            CreatedAt = DateTime.UtcNow,
            Files = new List<MediaFile>()
        };
        
        var archiveWithFiles = CreateMediaWithFile(@event, type: MediaType.Archive);
        
        await Db.Media.AddRangeAsync(archiveWithoutFiles, archiveWithFiles);
        await Db.SaveChangesAsync();

        // Act
        var client = await GetClientWithAuthAsync(user);
        var response = await client.GetAsync($"/api/Events/{@event.Id}/Media/Archives");

        // Assert - Should return the archive with files
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var archive = await response.Content.ReadFromJsonAsync<MediaModel?>();
        Assert.NotNull(archive);
        Assert.Equal(archiveWithFiles.Id, archive.Id);
    }

    // ========== Helper Methods ==========

    private Media CreateMediaWithFile(
        Event @event,
        MediaType type = MediaType.UserUpload,
        MediaStatus? status = MediaStatus.Validated,
        DateTime? createdAt = null)
    {
        var media = new Media
        {
            Event = @event,
            EventId = @event.Id,
            Type = type,
            Status = status,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            Files = new List<MediaFile>()
        };
        
        var mediaFile = new MediaFile
        {
            Media = media,
            MediaId = media.Id,
            Path = "/test/file",
            MimeType = "application/octet-stream",
            FileSize = 1024,
            FileType = MediaFileType.Original,
        };
        
        media.Files.Add(mediaFile);
        return media;
    }
}
