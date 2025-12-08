using AutoMapper;
using ERP.Core.Collections;

namespace ERP.Core.AutoMapper;

public class DefaultMappings : Profile
{
    public DefaultMappings()
    {
        CreateMap(typeof(PagedList<>), typeof(IPagedList<>))
            .ConvertUsing(typeof(PagedListConverter<,>));
    }
}