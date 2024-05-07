using System;
using System.Collections.Generic;
using System.Linq;

namespace Simacek.PagedList
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int TotalCount { get; }
        public int PageCount { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }
        public int StartPage { get; }
        public int EndPage { get; }
        public int PagerRange { get; set; }

        public PagedList(IQueryable<T> source, int? page, int pageSize = 10, int pagerRange = 10)
        {
            var totalCount = source.Count();
            var pageCount = (int)Math.Ceiling((decimal)totalCount / (decimal)pageSize);
            var currentPage = page ?? 1;
            var startPage = currentPage - (pagerRange / 2);
            var endPage = pagerRange % 2 != 0 ? (currentPage + (int)Math.Ceiling((double)pagerRange / 2.0)) : (currentPage + ((pagerRange / 2) - 1));

            if (startPage < 1)
            {
                endPage += (1 - startPage);
                startPage = 1;
            }

            if (endPage > pageCount)
            {
                endPage = pageCount;
                if (endPage > pagerRange)
                {
                    startPage = pagerRange % 2 != 0 ? (endPage - (pagerRange)) : (endPage - (pagerRange - 1));
                }
            }

            TotalCount = totalCount;
            PageCount = pageCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            StartPage = startPage;
            EndPage = endPage;
            PagerRange = pagerRange;

            this.AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList());
        }
    }
}
