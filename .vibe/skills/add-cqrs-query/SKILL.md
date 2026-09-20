---
name: add-cqrs-query
description: Load this skill when adding or modifying a CQRS query (read endpoint) in EventPhotographer — query records, response models, query handlers, controller endpoints, and their integration tests.
---

# Add a CQRS Query

Use when implementing a new read feature (list, detail, lookup) served by a controller endpoint. Complex feature logic belongs in CQRS queries, not in scattered service methods.

## File layout

Everything for one query lives in a single file under `EventPhotographer.UseCases/[Feature]/Queries/`:

- Query record(s)
- Response model records
- Handler class

## Steps

### 1. Define the query record

```csharp
public record GetMediaListForEventQuery : PagedQuery, IQuery<PagedResult<MediaModel>>
{
    public required Event Event;   // aggregate context, loaded by the controller
    public required User User;     // always pass the user; handlers authorize
}
```

- Non-paginated: `IQuery<TResult>` (e.g. `IQuery<MediaModel?>`)
- Paginated: inherit `PagedQuery` and declare `IQuery<PagedResult<TModel>>`

### 2. Define response models in the same file

Plain records, no entity references. Project inside the handler with `.Select(...)` so EF translates the whole query to SQL — never return entities from a query.

### 3. Implement the handler

```csharp
public class MediaQueryHandler(
    AppDbContext db,
    AuthorizationService authorizationService)
    : IQueryHandler<GetMediaListForEventQuery, PagedResult<MediaModel>>
{
    public async Task<Result<PagedResult<MediaModel>>> QueryAsync(GetMediaListForEventQuery query, CancellationToken cancellationToken = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(query.User, query.Event, new ManageEventRequirement());
        if (!authResult.IsAuthorized)
        {
            return authResult;   // implicit conversion to a failed Result<T>
        }

        var queryable = db.Media
            .Where(m => m.EventId == query.Event.Id);

        return await SelectMediaModel(queryable).ToPagedResultAsync(query, cancellationToken);
    }
}
```

Rules:

- Primary-constructor DI: `AppDbContext`, `AuthorizationService` — no repositories.
- Authorize first, before touching EF. Return `authResult` directly on failure (implicit conversion exists).
- Success: return the value (implicit conversion) or `Result.Success(value)`.
- Order lists deterministically before paginating: primary sort plus a `.ThenBy(x => x.Id)` tiebreaker. Without it, Postgres may return tied rows in any order per query and pages can skip or duplicate rows.
- Paginated lists: end with `await SelectModel(queryable).ToPagedResultAsync(query, cancellationToken);` — it clamps `Page >= 1` and `PageSize` to 1..50.

### 4. Add the controller endpoint

- Resolve the handler per-endpoint: `[FromServices] IQueryHandler<TQuery, TResult> queryHandler` — not constructor injection.
- Load the aggregate (e.g. `eventService.GetByIdAsync(eventId)`) and the user first; return `NotFound()` if the entity is missing.
- Map failures: `if (!result.IsSuccess) return result.ToProblemDetailsResult();`
- Success: `return Ok(result.Value);`
- Paginated endpoints bind `[FromQuery] PaginationQueryParameters` (`App/Common/DTO/QueryParams`) and map `Page`/`PageSize` onto the query. Do not bind `PagedQuery` directly in the controller — the DTO duplication is intentional layering.

### 5. Integration tests

Follow the feature-testing skill: `EventPhotographer.Tests/App/[Feature]/[Controller]Tests.cs`, Arrange-Act-Assert, Bogus fakers, `GetClientWithAuthAsync`. Cover at minimum:

- 404 for a nonexistent entity
- 404 for a non-owner (authorization failure surfaces as NotFound)
- OK with empty results
- OK with data, asserting on the response model
- For paginated queries: default parameters, clamped invalid parameters (`page=0&pageSize=0`), page navigation, and metadata (`TotalCount`, `TotalPages`, `HasNextPage`)
