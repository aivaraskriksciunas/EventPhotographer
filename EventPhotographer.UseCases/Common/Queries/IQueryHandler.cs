namespace EventPhotographer.UseCases.Common.Queries;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
    where TResult : class?
{
    public Task<Result<TResult>> QueryAsync(TQuery query, CancellationToken cancellationToken = default);
}
