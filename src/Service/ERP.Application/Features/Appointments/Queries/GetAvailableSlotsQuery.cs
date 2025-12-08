using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Appointments.Queries;

public abstract class GetAvailableSlotsQuery : IQuery<Result<List<TimeSlotDto>>>
{
    public Guid DoctorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}