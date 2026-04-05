namespace EcommercePos.Shared.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageIndex > 0;
    public bool HasNext => PageIndex < TotalPages - 1;
    public int FirstItemIndex => PageIndex * PageSize + 1;
    public int LastItemIndex => Math.Min((PageIndex + 1) * PageSize, TotalCount);

    public PagedResult()
    {
    }

    public PagedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}

public class PagedResult<T, TMeta> where TMeta : class
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public TMeta? Metadata { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageIndex > 0;
    public bool HasNext => PageIndex < TotalPages - 1;
}
