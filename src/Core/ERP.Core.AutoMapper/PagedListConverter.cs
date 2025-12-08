using AutoMapper;
using ERP.Core.Collections;

namespace ERP.Core.AutoMapper;

public class PagedListConverter<TSource, TDest> : ITypeConverter<PagedList<TSource>, IPagedList<TDest>>
    where TSource : class
    where TDest : class
{
    public IPagedList<TDest> Convert(PagedList<TSource> source, IPagedList<TDest> destination, ResolutionContext context)
    {
        var items = context.Mapper.Map<IList<TDest>>(source.Items);
        return new PagedList<TDest>(items, source.TotalCount);
    }
}