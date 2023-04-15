using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Helper
{
    public class PaginatedList<T> : List<T>
    {
        public int? PageIndex { get; private set; }
        public int? TotalPages { get; private set; }

        public PaginatedList(List<T> items, int? count, int? pageIndex, int? pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int?)Math.Ceiling(Convert.ToDouble(count) / Convert.ToDouble(pageSize));

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
        public bool IsLastPage => PageIndex != TotalPages;

        public static PaginatedList<T> CreateAsync(IQueryable<T>? source, int pageIndex, int pageSize)
        {
            var count =  source.Count();
            var items =  source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var a =  new PaginatedList<T>(items, count, pageIndex, pageSize);
            return a;
        }
    }
}
