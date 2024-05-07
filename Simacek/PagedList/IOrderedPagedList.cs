using Simacek.Linq;

namespace Simacek.PagedList
{
    public interface IOrderedPagedList<T> : IPagedList<T>
    {
        string CurrentSort { get; }
        Order CurrentOrder { get; }
    }
}
