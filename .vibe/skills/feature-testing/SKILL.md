---
name: feature-testing
description: Project-specific patterns for writing feature tests in EventPhotographer
instructions: |
  ## File Structure
  - Path: `EventPhotographer.Tests/App/[Feature]/[Controller]Tests.cs`
  - Namespace: `EventPhotographer.Tests.App.[Feature]`
  - Inherit from `BaseIntegrationTest`
  - Constructor: `public Tests(AppWebApplicationFactory factory) : base(factory)`

  ## Base Class Members
  - `Client` - HttpClient (unauthenticated)
  - `Db` - AppDbContext
  - `UserManager` - Identity UserManager
  - `CreateUserAsync(email?, password="Secret!123")` - Create registered user
  - `GetClientWithAuthAsync(User?)` - Authenticated HttpClient

  ## Pattern
  Use `[Fact]` or `[Theory]` with Arrange-Act-Assert.

  **Prefer `[Theory]` with `[MemberData]` to reduce duplication:**
  ```csharp
  [Theory]
  [MemberData(nameof(GetInvalidData))]
  public async Task Method_Scenario_ExpectedResult(object input)
  {
      // Act & Assert
  }

  public static IEnumerable<object[]> GetInvalidData() => new List<object[]>
  {
      new object[] { /* case 1 */ },
      new object[] { /* case 2 */ },
  };
  ```

  Use `[InlineData]` for simple values.

  ## Test Data Setup
  Use Bogus Fakers from `EventPhotographer.Tests.Fakes.[Feature]`:
  ```csharp
  var user = await CreateUserAsync();
  var @event = new EventFaker()
      .Rules((f, e) => e.User = user)
      .Generate();
  await Db.Events.AddAsync(@event);
  await Db.SaveChangesAsync();
  ```

  For entities with relationships, set both navigation and FK properties:
  ```csharp
  var media = new Media
  {
      Event = @event, EventId = @event.Id,
      Type = MediaType.UserUpload,
      Files = new List<MediaFile> { new MediaFile { /* fields */ } }
  };
  await Db.Media.AddRangeAsync(/* ... */);
  await Db.SaveChangesAsync();
  ```

  ## Requests
  ```csharp
  // Unauthenticated
  var response = await Client.GetAsync(url);
  
  // Authenticated
  var client = await GetClientWithAuthAsync(user);
  var response = await client.GetAsync(url);
  
  // Read JSON
  var result = await response.Content.ReadFromJsonAsync<T>();
  var nullable = await response.Content.ReadFromJsonAsync<T?>();
  ```

  ## Common Scenarios
  - 404 for non-existent entity
  - 401 for unauthenticated
  - 404 for unauthorized (current implementation returns NotFound)
  - OK with empty list/null for no results
  - OK with data for valid requests

  ## Tips
  - Use strongly-typed DTOs (e.g., `MediaModel`) over `dynamic`
  - Use `[Theory]` + `[MemberData]`/`[InlineData]` to minimize repetition
  - Test both success and error paths
  - Reference: `EventMediaControllerTests.cs`, `EventsTests.cs`

  ## When to Ask
  Clarify expected behavior, authorization rules, or status codes if unsure.

  ## Running Tests
  When this skill is active, you can execute test commands. Run from project root:
  ```bash
  dotnet test EventPhotographer.Tests -v q
  ```

  Filter to current skill's test class:
  ```bash
  dotnet test --filter "FullyQualifiedName~EventPhotographer.Tests.App.Events.EventMediaTests"
  ```

  Filter to specific test method:
  ```bash
  dotnet test --filter "Name~ListMedia_EventOwnerWithMedia"
  ```

  Filter operators: `=` (exact), `~` (contains), `!` (not), `&` (and), `|` (or)

labels: ["testing", "csharp", "xunit", "eventphotographer"]
---
