using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.CreateSchedule;

public class CreateScheduleCommand : IRequest<Result<Guid>>
{
    public Guid DoctorId { get; set; }
    public int DayOfWeek { get; set; } // Uses System.DayOfWeek int (0-6)
    public string StartTime { get; set; } = string.Empty; // Format HH:mm:ss
    public string EndTime { get; set; } = string.Empty;   // Format HH:mm:ss
    public int SlotDurationMinutes { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsRecurring { get; set; } = true;
}