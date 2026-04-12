using EcommercePos.Shared.Common;

namespace EcommercePos.Shared.Common;

public static class QueryParams
{
    public static int DefaultPageSize => 10;
    public static int MaxPageSize => 100;
}

public class PaginationParams
{
    private int _pageSize = QueryParams.DefaultPageSize;
    private int _pageIndex = 0;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 0 ? 0 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > QueryParams.MaxPageSize ? QueryParams.MaxPageSize : value;
    }

    public int Skip => PageIndex * PageSize;

    public PaginationParams()
    {
    }

    public PaginationParams(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}

