---
name: add-worker-message
description: Load this skill when adding or modifying a message sent from the API to the EventPhotographer Worker via RabbitMQ/EasyNetQ — message records, publishing, consumers, Hangfire job wiring, and their DI registration.
---

# Add Worker Message

Recipe for wiring a message from the API to the Worker, mirroring the existing `ValidateUploadedFileMessage` pipeline. All five steps are required — a missing consumer registration compiles fine and fails silently at runtime.

```
Message record in Core/Features/[Feature]/Messages/
→ published via IBus.PubSub.PublishAsync (after SaveChangesAsync)
→ consumer in Worker/Consumers implementing IConsumeAsync<T>
→ consumer enqueues a Hangfire job
→ job executes with the entity id as parameter
```

1. **Message record** — `EventPhotographer.Core/Features/[Feature]/Messages/[Name]Message.cs`. Minimal DTO: just the entity id. The Worker re-reads the entity from the DB; never put entity state in the message.

2. **Publish** — inject `IBus` where the entity is created, publish **after** `SaveChangesAsync` (the consumer would otherwise read an uncommitted row):
   ```csharp
   await bus.PubSub.PublishAsync(new [Name]Message { [Entity]Id = entity.Id });
   ```

3. **Consumer** — `EventPhotographer.Worker/Consumers/[Name]MessageConsumer.cs` implementing `IConsumeAsync<T>`. Simple logic can live directly in the consumer, but anything heavier should be moved to a job: if consumption runs long, RabbitMQ ack-times out and requeues the message, re-executing it. `IBackgroundJobClient.Enqueue` for immediate execution, `.Schedule` for a delay (see `ValidateUploadedFileMessageConsumer`).

4. **Register the consumer** in `AddWorkerConsumers` (`EventPhotographer.Worker/DependencyInjection.cs`). Subscription is automatic — the Worker's `RegisterMessageConsumers` hosted service AutoSubscribes all `IConsumeAsync<T>` in the Worker assembly (subscription id `"worker"`; the API's uses `"api"`, so the two sides never subscribe each other's consumers). Do not add Worker consumers to the API's `AddWorkerConsumers` in `EventPhotographer/Core/Setup.cs` — despite the name, that one is for API-side consumers like `UploadedFileNotificationConsumer`.

5. **Job** — give the job a parameterized `ExecuteAsync(Guid id)` overload. Reuse existing reservation/query helpers via an optional parameter instead of duplicating; keep existing lock names unchanged (the lock name is the concurrency contract). Guard the reservation with the status check so a redelivered message is a no-op.

## Gotchas

- **Integration tests have no RabbitMQ.** A new publish site fails tests with a misleading **405 MethodNotAllowed** (the exception handler re-executes the original POST against `/Error`), not a 500. `AppWebApplicationFactory` already replaces `IBus` with a mock — if a test suddenly 405s after adding a publish, it is this.
- If re-mocking `IBus`: `Mock.Of<IBus>()` returns null for `bus.PubSub`. Use `new Mock<IBus> { DefaultValue = DefaultValue.Mock }.Object`.
