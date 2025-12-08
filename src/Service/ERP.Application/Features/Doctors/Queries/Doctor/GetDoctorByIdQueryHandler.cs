using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Specifications.Doctor;
using MediatR;

namespace ERP.Application.Features.Doctors.Queries.Doctor;

public class GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository, IObjectMapper mapper) : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.FirstOrDefaultAsync(new GetDoctorByIdSpecification(request.Id), cancellationToken);

        if (doctor is null)
            return Result.Failure<DoctorDto>("Doctor does not exist");
        
        return Result.Success(mapper.Map<DoctorDto>(doctor));
    }
}