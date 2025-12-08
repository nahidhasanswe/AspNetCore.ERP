using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Collections;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Specifications.Clinic;
using MediatR;

namespace ERP.Application.Features.Clinics.Queries;

public class GetClinicPaginationListHandler(IClinicRepository clinicRepository, IObjectMapper mapper)
    : IRequestHandler<GetClinicPaginationListQuery, PaginationResult<ClinicDto>>
{
    public async Task<PaginationResult<ClinicDto>> Handle(GetClinicPaginationListQuery request, CancellationToken cancellationToken)
    {
        var searchSpecification = new GetAllClinicWithPaginationSpecification
        (
            request.Search,
            request.City,
            request.Country,
            request.Active,
            request.PageIndex,
            request.PageSize,
            request.Sort
        );
        
        var result = await clinicRepository.GetPaginationListAsync(searchSpecification, cancellationToken);
        
        return Result.SuccessForPagination(mapper.Map<IPagedList<ClinicDto>>(result));
    }
}