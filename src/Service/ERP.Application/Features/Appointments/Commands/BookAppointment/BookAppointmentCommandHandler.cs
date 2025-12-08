using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<BookAppointmentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Verify patient exists
        var patient = await patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result.Failure<Guid>("Patient not found");

        // Get time slot with optimistic concurrency check
        var timeSlot = await appointmentRepository.GetTimeSlotByIdAsync(request.TimeSlotId, cancellationToken);
        if (timeSlot is null)
            return Result.Failure<Guid>("Time slot not found");

        // Check if slot is available
        if (timeSlot.Status != SlotStatus.Available)
            return Result.Failure<Guid>("Time slot is not available");

        // Check if slot is in the future
        if (timeSlot.SlotStartDateTime <= DateTime.UtcNow)
            return Result.Failure<Guid>("Cannot book past time slots");

        try
        {
            // Book the time slot (with optimistic concurrency)
            var bookResult = timeSlot.Book();
            if (bookResult.IsFailure)
                return Result.Failure<Guid>(bookResult.Error);

            appointmentRepository.UpdateTimeSlot(timeSlot);

            // Create appointment
            var appointment = new Appointment(
                timeSlot.Id,
                request.PatientId,
                timeSlot.DoctorId,
                timeSlot.Doctor.ClinicId,
                request.Notes);
            
            using var scope = unitOfWork.Begin();

            await appointmentRepository.AddAsync(appointment, cancellationToken);
            await scope.SaveChangesAsync(cancellationToken);

            return Result.Success(appointment.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure<Guid>("Time slot was already booked by another user. Please select another slot.");
        }
    }
}