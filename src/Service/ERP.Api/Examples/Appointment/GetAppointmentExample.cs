using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Appointment;

public class GetAppointmentExample : CustomExampleProvider<AppointmentDto>
{
    public override ApiResponse<AppointmentDto> GetExamples()
    {
        var model = new AppointmentDto
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            PatientName = "Emily Brown",
            DoctorId = Guid.NewGuid(),
            DoctorName = "Dr. John Doe",
            Specialization = "Cardiology",
            ClinicId = Guid.NewGuid(),
            ClinicName = "Central Clinic",
            AppointmentDateTime = DateTime.UtcNow.AddDays(7).Date.AddHours(10),
            AppointmentEndDateTime = DateTime.UtcNow.AddDays(7).Date.AddHours(10).AddMinutes(30),
            Status = "Scheduled",
            BookingDate = DateTime.UtcNow,
            Notes = "Patient requested morning appointment."
        };

        return GetResponse(model);
    }
}