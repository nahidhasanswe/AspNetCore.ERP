using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.ClinicAggregate;
using MediatR;

namespace ERP.Application.Features.Clinics.Queries;

public class GetClinicByIdQueryHandler(IClinicRepository clinicRepository, IObjectMapper mapper)
    : IRequestHandler<GetClinicByIdQuery, Result<ClinicDto>>
{
    public async Task<Result<ClinicDto>> Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.Id, cancellationToken);

        if (clinic is null)
            return Result.Failure<ClinicDto>("Clinic not found.");
        
        return Result.Success(mapper.Map<ClinicDto>(clinic));
    }
}