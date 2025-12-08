using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Doctors.Queries.Doctor;

public sealed class GetDoctorByIdQuery(Guid id): IQuery<Result<DoctorDto>>
{
    public Guid Id => id;
}