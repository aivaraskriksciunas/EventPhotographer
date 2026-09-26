using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core.Features.Emails.Messages;
using EventPhotographer.Worker.Jobs;
using Hangfire;

namespace EventPhotographer.Worker.Consumers;

internal class SendEmailMessageConsumer(
    IBackgroundJobClient backgroundJobs)
    : IConsumeAsync<SendEmailMessage>
{
    public Task ConsumeAsync(SendEmailMessage message, CancellationToken cancellationToken = default)
    {
        backgroundJobs.Enqueue<SendScheduledEmailsJob>(m => m.ExecuteAsync(message.EmailId));

        return Task.CompletedTask;
    }
}
