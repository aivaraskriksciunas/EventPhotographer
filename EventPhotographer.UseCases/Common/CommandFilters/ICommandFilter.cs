using EventPhotographer.UseCases.Common.Commands;

namespace EventPhotographer.UseCases.Common.PipelineBehaviours;

internal interface ICommandFilter
{
    public Task<Result<TResult>?> ApplyFilterAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResult>;
}