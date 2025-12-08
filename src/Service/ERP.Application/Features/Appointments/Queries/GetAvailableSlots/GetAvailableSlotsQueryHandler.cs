using ERP.Application.DTOs;
using ERP.Core;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using MediatR;

namespace ERP.Application.Features.Appointments.Queries.GetAvailableSlots;

public class GetAvailableSlotsQueryHandler(
    IAppointmentRepository appointmentRepository,
    IDoctorRepository doctorRepository)
    : IRequestHandler<GetAvailableSlotsQuery, Result<List<TimeSlotDto>>>
{
    public async Task<Result<List<TimeSlotDto>>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
            return Result.Failure<List<TimeSlotDto>>("Doctor not found");

        var slots = await appointmentRepository.GetAvailableSlotsAsync(
            request.DoctorId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var slotDtos = slots.Select(s => new TimeSlotDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DoctorName = doctor.FullName,
            SlotDateTime = s.SlotStartDateTime,
            Status = s.Status.ToString(),
            DurationMinutes = s.Schedule.SlotDurationMinutes
        }).ToList();

        return Result.Success(slotDtos);
    }
}