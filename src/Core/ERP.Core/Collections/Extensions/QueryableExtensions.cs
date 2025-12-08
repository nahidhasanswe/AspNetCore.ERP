using System.Linq;

namespace ERP.Core.Collections.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Get the List by Pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageNo">Deafult value 0</param>
        /// <param name="pageSize">Deafult value 50</param>
        /// <returns><typeparam name="T"> Queryable result</returns>
        public static IQueryable<T> ToPagination<T>(this IQueryable<T> source,int pageNo=0, int pageSize = 50)
        {
            return source.Skip(pageNo * pageSize).Take(pageSize);
        } 
    }
}
