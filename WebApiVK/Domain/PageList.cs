namespace WebApiVK.Domain;

public class PageList<T> : List<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public long TotalCount { get; }

    public PageList(List<T> items, long totalCount, int currentPage, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        AddRange(items);
    }
}