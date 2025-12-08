using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid TimeSlotId { get; set; }
    public Guid PatientId { get; set; }
    public string? Notes { get; set; }
}