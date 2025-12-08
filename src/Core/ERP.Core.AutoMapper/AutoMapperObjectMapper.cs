using ERP.Core.Mapping;
using AutoMapper;

namespace ERP.Core.AutoMapper;

public class AutoMapperObjectMapper(IMapper mapper) : IObjectMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return mapper.Map(source, destination);
    }

    public IQueryable<TDestination> GetProjection<TSource, TDestination>(IQueryable<TSource> source)
    {
        return mapper.ProjectTo<TDestination>(source);
    }
}