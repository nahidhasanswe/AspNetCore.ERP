using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.PatientAggregate;
using MediatR;

namespace ERP.Application.Features.Patient.Queries;


public class GetPatientByIdHandler(IPatientRepository patientRepository, IObjectMapper mapper)
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (patient is null)
            return Result.Failure<PatientDto>("Patient not found.");
        
        return Result.Success(mapper.Map<PatientDto>(patient));
    }
}