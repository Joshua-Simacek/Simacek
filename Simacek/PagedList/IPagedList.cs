using System.Collections.Generic;

namespace Simacek.PagedList
{
    public interface IPagedList<T> : IList<T>
    {
        int TotalCount { get; }
        int PageCount { get; }
        int PageSize { get; }
        int CurrentPage { get; }
        int StartPage { get; }
        int EndPage { get; }
        int PagerRange { get; }
    }
}
