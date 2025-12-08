using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Aggregates.PatientAggregate;
using MediatR;

namespace ERP.Application.Features.Appointments.Queries.Appointments;

public class GetAppointmentsByPatientQueryHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IObjectMapper mapper)
    : IRequestHandler<GetAppointmentsByPatientQuery, Result<List<AppointmentDto>>>
{
    public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsByPatientQuery request, CancellationToken cancellationToken)
    {
        var patientExists = await patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patientExists is null)
            return Result.Failure<List<AppointmentDto>>("Patient not found.");

        // The repository is responsible for fetching Appointments and necessary includes (TimeSlot, Doctor, Patient, Clinic)
        var appointments = await appointmentRepository.GetByPatientIdAsync(request.PatientId, cancellationToken);

        return Result.Success(mapper.Map<List<AppointmentDto>>(appointments));
    }
}