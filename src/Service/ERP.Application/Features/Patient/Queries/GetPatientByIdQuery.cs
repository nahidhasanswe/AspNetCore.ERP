using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Patient.Queries;

public sealed class GetPatientByIdQuery(Guid id) : IQuery<Result<PatientDto>>
{
    public Guid Id => id;
}