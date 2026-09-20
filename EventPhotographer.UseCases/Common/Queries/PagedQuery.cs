namespace EventPhotographer.UseCases.Common.Queries;

public record PagedQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
