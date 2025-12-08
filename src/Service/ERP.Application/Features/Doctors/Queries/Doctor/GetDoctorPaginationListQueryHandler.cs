using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Collections;
using MediatR;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Specifications.Doctor;

namespace ERP.Application.Features.Doctors.Queries.Doctor;

public class GetDoctorPaginationListQueryHandler(IDoctorRepository doctorRepository, IObjectMapper mapper)
    : IRequestHandler<GetDoctorPaginationListQuery, PaginationResult<DoctorDto>>
{
    public async Task<PaginationResult<DoctorDto>> Handle(GetDoctorPaginationListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetDoctorListWithPaginationSpecification(
            request.Search,
            request.Specialization,
            request.ClinicId,
            request.Active,
            request.PageIndex,
            request.PageSize,
            request.Sort
        );

        var result = await doctorRepository.GetPaginationListAsync(spec, cancellationToken);
        return Result.SuccessForPagination<DoctorDto>(mapper.Map<IPagedList<DoctorDto>>(result));
    }
}
