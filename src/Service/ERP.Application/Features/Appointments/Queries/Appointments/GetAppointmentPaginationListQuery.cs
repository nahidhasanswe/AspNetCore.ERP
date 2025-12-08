using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Domain.Enums;

namespace ERP.Application.Features.Appointments.Queries.Appointments;

public sealed class GetAppointmentPaginationListQuery : IPaginationQuery<PaginationResult<AppointmentDto>>
{
    public Guid? PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? ClinicId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AppointmentStatus? Status { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 50;
    public string? Sort { get; set; }
}
