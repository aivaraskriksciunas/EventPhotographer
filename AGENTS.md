# EventPhotographer - Application Overview

This is a project for an image sharing site. Users create events and share those events with others. Visitors upload images and videos of the event. After the event, a zip file is generated with the uploaded content and made available to the organizer.

## Architecture

Clean Architecture with domain-centric design:

```
EventPhotographer/          - ASP.NET Core Web API (main entry point)
EventPhotographer.Core/     - Domain layer: entities, DbContext, features
EventPhotographer.UseCases/ - Application layer: commands, queries, CQRS
EventPhotographer.Worker/   - Background worker (RabbitMQ consumers, jobs)
eventphotographer.client/   - React/Vite frontend
```

## Core Features

### Events
- Event is created by logged in users. 
- Other users upload Media to events.

### Media
- Tied to an event
- Uploaded by logged in or anonymous user
- Content is stored on S3 compatible storage. Database stores only URL to it.
- S3 URL should never be exposed to outside - we are using API for serving it in order to verify permissions.

### Participant
- Tied to a specific event
- Can be tied to a user or be anonymous
- Uploads Media to Events

### User 
- ASP Identity User

## Infrastructure

- **Database**: PostgreSQL via Entity Framework Core
- **Message Queue**: RabbitMQ with EasyNetQ/MassTransit
- **Object Storage**: Configurable storage for media files
- **Containerization**: Docker with docker-compose
- **Monitoring**: Sentry integration

## Worker Services

Background processing for:
- WhatsApp webhook payload processing
- File validation and compression
- Scheduled recurring jobs
 
Communicates with the main entry point via RabbitMQ. Runs on a separate machine and must not contain any direct connection with the other parts.

## Frontend

React + TypeScript + Vite:
- Feature-based organization (events, auth, public-events)
- Makes API requests to main entrypoint.
- Router-based navigation

## Key Patterns

- CQRS with command/query separation. 
- API endpoints are handled by Controllers. Complex feature logic must be moved to CQRS, instead of being split into scattered service methods.
- No repositories - using EF Core in Services or CQRS commands and queries. Avoid using EF outside of these scopes. 
- Filter pipeline for commands (auth, validation)
- Dependency injection throughout

## Implementation Details

### Entities & Database
- Enums are stored as strings using `.HasConversion<string>()` with optional max length
- Entity mappings use `EntityTypeConfiguration<T>` classes, typically inheriting from `UUIDEntityConfiguration<T>` for GUID primary keys
- Bidirectional relationships require setting both navigation property and foreign key (e.g., `MediaFile.Media` and `MediaFile.MediaId`)

### Authorization
- `ManageEventRequirement` - Only the event owner can access (used by EventMediaController)
- `ViewMediaRequirement` - Participant who uploaded the media OR event owner can access

### Result Pattern
- Queries return `Result<T>` with `IsSuccess` and `Value` properties
- Controllers use `result.ToProblemDetailsResult()` to convert failures to HTTP problem details
- Success: `return Ok(result.Value)`
- Entity not found: `return NotFound()`

### CQRS Queries
- Query records, response models, and their handler live together in one file under `UseCases/[Feature]/Queries`
- Handlers implement `IQueryHandler<TQuery, TResult>`; controllers resolve them per-endpoint via `[FromServices]`, not constructor injection

### Pagination
- Paginated queries inherit `PagedQuery` and return `PagedResult<T>` (`Items`, `Page`, `PageSize`, `TotalCount`, `TotalPages`, `HasPreviousPage`, `HasNextPage`)
- Handlers call `queryable.ToPagedResultAsync(query, ct)` from `UseCases/Common/Queries/PaginationExtensions`, which clamps `Page >= 1` and `PageSize` to 1..50
- Order the queryable deterministically before paginating (tie-break by `Id`), otherwise pages can skip or duplicate rows
- Controllers bind `[FromQuery] PaginationQueryParameters` (`App/Common/DTO/QueryParams`) and map it onto the query — the DTO duplication with `PagedQuery` is intentional layering
- Frontend: `PagedResponse<T>` and `PaginationQueryParameters` in `src/api/client.ts`; reuse `PaginatedView` + `LoadMorePaginator` from `src/components/pagination` (render prop + context). Keep the page size identical between the route loader's initial fetch and subsequent fetches

### Testing
- xUnit integration tests in `EventPhotographer.Tests`, inheriting `BaseIntegrationTest` (provides `Client`, `Db`, `CreateUserAsync`, `GetClientWithAuthAsync`)
- Tests run against a real PostgreSQL via Testcontainers — Docker must be running
- Run all: `dotnet test EventPhotographer.Tests -v q`; filter: `dotnet test --filter "FullyQualifiedName~EventPhotographer.Tests.App.Events.EventMediaTests"`
- Test data is built with Bogus fakers from `EventPhotographer.Tests/Fakes`