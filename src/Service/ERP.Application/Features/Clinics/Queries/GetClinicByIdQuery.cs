using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Clinics.Queries;

public sealed class GetClinicByIdQuery(Guid id) : IQuery<Result<ClinicDto>>
{
    public Guid Id { get; private set; } = id;
}