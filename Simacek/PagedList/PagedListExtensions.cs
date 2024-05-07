using System.Linq;
using Simacek.Linq;

namespace Simacek.PagedList
{
    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int? page, int pageSize = 10)
        {
            return new PagedList<T>(source, page, pageSize);
        }

        //public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int? page, int pageSize = 10)
        //{
        //    return new PagedList<T>(source, page, pageSize);
        //}

        public static IOrderedPagedList<T> ToOrderedPagedList<T>(this IQueryable<T> source, string orderBy, Order orderDirection, int? page, int pageSize = 10)
        {
            return new OrderedPagedList<T>(source, orderBy, orderDirection, page, pageSize);
        }

        //public static IOrderedPagedList<T> ToOrderedPagedList<T>(this IEnumerable<T> source, string orderBy, Order orderDirection, int? page, int pageSize = 10)
        //{
        //    return new OrderedPagedList<T>(source, orderBy, orderDirection, page, pageSize);
        //}
    }
}
