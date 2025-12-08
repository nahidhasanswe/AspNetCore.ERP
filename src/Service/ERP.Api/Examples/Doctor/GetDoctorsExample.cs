using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;
using System.Collections.Generic;

namespace ERP.Api.Examples.Doctor;

public class GetDoctorsExample : CustomExampleProvider<List<DoctorDto>>
{
    public override ApiResponse<List<DoctorDto>> GetExamples()
    {
        var doctors = new List<DoctorDto>
        {
            new DoctorDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                FullName = "John Doe",
                Specialization = "Cardiology",
                LicenseNumber = "MD12345",
                Phone = "555-123-4567",
                Email = "john.doe@example.com",
                ClinicId = Guid.NewGuid(),
                ClinicName = "Central Clinic",
                IsActive = true
            },
            new DoctorDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                FullName = "Jane Smith",
                Specialization = "Pediatrics",
                LicenseNumber = "MD67890",
                Phone = "555-987-6543",
                Email = "jane.smith@example.com",
                ClinicId = Guid.NewGuid(),
                ClinicName = "Children's Hospital",
                IsActive = true
            }
        };

        return GetResponse(doctors);
    }
}