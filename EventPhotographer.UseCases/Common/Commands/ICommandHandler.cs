namespace EventPhotographer.UseCases.Common.Commands;

public interface ICommandHandler<in TCommand, TResult> 
    where TCommand : class, ICommand<TResult>
    where TResult : class
{
    public Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

