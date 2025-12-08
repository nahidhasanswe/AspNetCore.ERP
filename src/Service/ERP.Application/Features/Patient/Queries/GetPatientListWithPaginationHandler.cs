using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Collections;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.Specifications.Patient;
using MediatR;

namespace ERP.Application.Features.Patient.Queries;

public class GetPatientListWithPaginationHandler(IPatientRepository patientRepository, IObjectMapper mapper)
    : IRequestHandler<GetPatientListWithPaginationQuery, PaginationResult<PatientDto>>
{
    public async Task<PaginationResult<PatientDto>> Handle(GetPatientListWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var searchSpecification = new GetPatientListWithPaginationSpecification
            (
                request.Search,
                request.Gender,
                request.City,
                request.Country,
                request.FromDateOfBirth,
                request.ToDateOfBirth,
                request.PageIndex,
                request.PageSize,
                request.Sort
            );
        
        var result = await patientRepository.GetPaginationListAsync(searchSpecification, cancellationToken);
        
        return Result.SuccessForPagination<PatientDto>(mapper.Map<IPagedList<PatientDto>>(result));
    }
}