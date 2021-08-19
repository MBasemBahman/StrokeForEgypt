using System;
using System.Collections.Generic;
using System.Linq;

namespace StrokeForEgypt.Service
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => (CurrentPage > 1);

        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }
        // Note Convert IEnumerable<T> => IQueryable<T>
        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }

    public class PaginationMetaData<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious { get; private set; }

        public bool HasNext { get; private set; }

        public string PrevoisPageLink { get; set; }

        public string NextPageLink { get; set; }

        public PaginationMetaData(PagedList<T> Result)
        {
            CurrentPage = Result.CurrentPage;
            TotalPages = Result.TotalPages;
            PageSize = Result.PageSize;
            TotalCount = Result.TotalCount;
            HasPrevious = Result.HasPrevious;
            HasNext = Result.HasNext;
        }
    }
}
