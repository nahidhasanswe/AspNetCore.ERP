using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.AppointmentAggregate;
using MediatR;

namespace ERP.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CancelAppointmentCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Failure("Appointment not found");

        var cancelResult = appointment.Cancel(request.Reason);
        if (cancelResult.IsFailure)
            return cancelResult;

        // Make the time slot available again
        var timeSlot = appointment.TimeSlot;
        var availableResult = timeSlot.MakeAvailable();
        if (availableResult.IsFailure)
            return availableResult;

        using var scope = unitOfWork.Begin();
        
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        appointmentRepository.UpdateTimeSlot(timeSlot);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}