---
name: add-cqrs-command
description: Load this skill when adding or modifying a CQRS command (write endpoint) in EventPhotographer — command records, error records, command handlers, the filter pipeline (authorization, validation), controller endpoints, and their integration tests.
---

# Add a CQRS Command

Use when implementing a write feature (create, update, state transition) served by a controller endpoint. Complex feature logic belongs in CQRS commands, not in scattered service methods.

## File layout

Everything for one command lives in a single file under `EventPhotographer.UseCases/[Feature]/Commands/`:

- Command record
- Error records (or `Errors.cs` in the same folder when shared across commands)
- Handler class (internal)

## Steps

### 1. Define the command record

```csharp
public record StartAccountVerification
    : ICommand<AccountVerification>
{
    public required User User { get; set; }
}
```

- `ICommand<TResult>` where `TResult` is the entity or result model returned on success.
- Pass the acting `User` as a property when the handler needs it (authorization, ownership, audit).
- For authorization via the filter pipeline, also implement `IRequiresAuthorization` and `IAuthorizationUserAware`, and expose requirements:

```csharp
public record CreateEvent : BaseEventEditCommand,
    IAuthorizationUserAware,
    IRequiresAuthorization
{
    public User User { get; set; } = null!;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new CreateEventRequirement();
    }
}
```

- Resource-based authorization: implement `IRequiresResourceAuthorization` and return the entity from `GetAuthorizationResource()` (see `CreateMediaCommand`).

### 2. Define error records

Failures carry typed errors, not strings. Records extending `Error` with a code; add payload properties when the caller needs detail:

```csharp
public record AccountVerificationNotAvailableError(DateTime RetryAt)
    : Error("AccountVerificationNotAvailable")
{ }
```

- Group shared errors in `Errors.cs` in the same folder.
- `ErrorType` defaults to `BusinessLogicError` (maps to 400 in `ToProblemDetailsResult`); use `Error.NotFound` for missing entities and `new AccessDeniedError(authResult)` for authorization failures.

### 3. Implement the handler

```csharp
internal class StartAccountVerificationHandler(
    AccountVerificationService accountVerificationService,
    TemplateEmailService emailService)
    : ICommandHandler<StartAccountVerification, AccountVerification>
{
    public async Task<Result<AccountVerification>> HandleAsync(
        StartAccountVerification command,
        CancellationToken cancellationToken = default)
    {
        var retryAt = await accountVerificationService
            .CalculateWhenNextVerificationCanBeGenerated(command.User);

        if (retryAt > DateTime.UtcNow)
        {
            return new AccountVerificationNotAvailableError(retryAt);
        }

        var verification = await accountVerificationService.CreateVerification(command.User);
        await emailService.ScheduleAccountVerificationEmail(command.User, verification);

        return verification;
    }
}
```

Rules:

- Primary-constructor DI: `AppDbContext`, feature services, `AuthorizationService` — no repositories.
- Handlers are auto-registered by the Scrutor scan in `UseCases/DependencyInjection.cs` (internal classes included). No manual registration.
- Guard clauses return error records; success returns the value (implicit conversion to `Result<T>`).
- Use `DateTime.UtcNow`, never `new DateTime()` (that is `DateTime.MinValue`) or `DateTime.Now`.
- Optional validation: add an internal `AbstractValidator<TCommand>` in the same file — `ValidationFilter` picks it up automatically.
- Core feature services used by the handler must be registered in `Core/Features/[Feature]/DependencyInjection.cs`; a missing registration compiles but throws at DI resolution.

### 4. Add the controller endpoint

```csharp
[HttpGet]
[Route("start-verification")]
[Authorize]
public async Task<ActionResult> StartVerification(
    [FromServices] ICommandHandler<StartAccountVerification, AccountVerification> commandHandler)
{
    var user = await userManager.GetUserAsync(User);
    if (user == null)
    {
        return Unauthorized();
    }

    var result = await commandHandler.HandleAsync(new StartAccountVerification
    {
        User = user,
    });

    if (!result.IsSuccess)
    {
        return result.ToProblemDetailsResult();
    }

    return Ok();
}
```

- Resolve the handler per-endpoint: `[FromServices] ICommandHandler<TCommand, TResult>` — not constructor injection.
- Controllers inherit `ApiController` (`api/[controller]`); override the class-level `[Route("api/[resource]")]` when the URL should not match the controller name (see `AccountVerificationController`).
- Resolve the current user via `userManager.GetUserAsync(User)` and pass it into the command.
- Failures: `return result.ToProblemDetailsResult();`. Success: `return Ok(result.Value)` — or `Ok()` when the response body must stay empty (e.g. the verification code is email-only and must never be returned over the API).

### 5. Integration tests

Follow the feature-testing skill: `EventPhotographer.Tests/App/[Feature]/[Controller]Tests.cs`, Arrange-Act-Assert, Bogus fakers, `GetClientWithAuthAsync`. Cover at minimum:

- Success path, asserting on the persisted state (commands mutate — verify via `Db`, not just the response)
- Each error branch with its expected status code (400 for business-logic errors, 404 for `Error.NotFound`)
- 401 when the endpoint requires authentication and the request is anonymous
- Authorization denial surfaces as 404 when `accessDeniedAsNotFound` applies
