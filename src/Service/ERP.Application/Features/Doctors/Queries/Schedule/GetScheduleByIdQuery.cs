using ERP.Application.DTOs;
using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Doctors.Queries.Schedule;

public abstract class GetScheduleByIdQuery : IRequest<Result<TimeSlotDto>>
{
    public Guid Id { get; set; }
}
