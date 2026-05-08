namespace ShopApp.Application.Common;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize);
