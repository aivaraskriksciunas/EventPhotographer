namespace EventPhotographer.App.Common.DTO.QueryParams;

public record PaginationQueryParameters
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
