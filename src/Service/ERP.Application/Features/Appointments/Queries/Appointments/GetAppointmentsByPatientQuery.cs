using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Appointments.Queries.Appointments;

public abstract class GetAppointmentsByPatientQuery : IQuery<Result<List<AppointmentDto>>>
{
    public Guid PatientId { get; set; }
}