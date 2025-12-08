using System.Collections.Generic;

namespace ERP.Core.Collections
{
    public interface IPagedList<T>
    {
        IList<T> Items { get; set; }

        int TotalCount { get; set; }
    }
}
