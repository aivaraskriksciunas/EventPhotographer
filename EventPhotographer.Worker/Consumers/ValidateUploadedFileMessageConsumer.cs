using EasyNetQ.AutoSubscribe;
using EventPhotographer.Core.Features.Content.Messages;
using EventPhotographer.Worker.Jobs;
using Hangfire;

namespace EventPhotographer.Worker.Consumers;

internal class ValidateUploadedFileMessageConsumer(
    IBackgroundJobClient backgroundJobs) 
    : IConsumeAsync<ValidateUploadedFileMessage>
{
    public Task ConsumeAsync(ValidateUploadedFileMessage message, CancellationToken cancellationToken = default)
    {
        if (message.DelayValidationMs > 0)
        {
            backgroundJobs.Schedule<ValidateUploadedFileJob>(
                m => m.ExecuteAsync(message.MediaFileId),
                TimeSpan.FromMilliseconds(message.DelayValidationMs));
        }
        else
        {
            backgroundJobs.Enqueue<ValidateUploadedFileJob>(m => m.ExecuteAsync(message.MediaFileId));
        }

        return Task.CompletedTask;
    }
}
