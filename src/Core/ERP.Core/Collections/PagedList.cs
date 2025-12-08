using System.Collections.Generic;

namespace ERP.Core.Collections
{
    public class PagedList<T> : IPagedList<T>
        where T : class
    {
        public PagedList()
        {
        }

        public PagedList(IList<T> list, int totalCount)
        {
            Items = list;
            TotalCount = totalCount;
        }

        public PagedList(IPagedList<T> list)
        {
            Items = list.Items;
            TotalCount = list.TotalCount;
        }

        public IList<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }
    }
}
