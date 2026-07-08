using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int) Math.Ceiling(TotalCount / (double) PageSize);
        public bool HasPreviousPage => PageNo > 1;
        public bool HasNextPage => PageNo < TotalPages;
    }
}
