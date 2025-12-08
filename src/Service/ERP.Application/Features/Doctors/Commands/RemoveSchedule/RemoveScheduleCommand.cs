using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.RemoveSchedule;

public class RemoveScheduleCommand : IRequest<Result<Guid>>
{
    public Guid DoctorId { get; set; }
    public Guid ScheduleId { get; set; }
}