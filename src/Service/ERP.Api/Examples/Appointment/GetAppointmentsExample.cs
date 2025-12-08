using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;
using System.Collections.Generic;

namespace ERP.Api.Examples.Appointment;

public class GetAppointmentsExample : CustomExampleProvider<List<AppointmentDto>>
{
    public override ApiResponse<List<AppointmentDto>> GetExamples()
    {
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto
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
            },
            new AppointmentDto
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Michael Green",
                DoctorId = Guid.NewGuid(),
                DoctorName = "Dr. Jane Smith",
                Specialization = "Pediatrics",
                ClinicId = Guid.NewGuid(),
                ClinicName = "Children's Hospital",
                AppointmentDateTime = DateTime.UtcNow.AddDays(8).Date.AddHours(14),
                AppointmentEndDateTime = DateTime.UtcNow.AddDays(8).Date.AddHours(14).AddMinutes(45),
                Status = "Scheduled",
                BookingDate = DateTime.UtcNow,
                Notes = "Follow-up for vaccination."
            }
        };

        return GetResponse(appointments);
    }
}
