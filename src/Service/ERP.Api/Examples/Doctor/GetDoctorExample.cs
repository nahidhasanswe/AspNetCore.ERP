using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Doctor;

public class GetDoctorExample : CustomExampleProvider<DoctorDto>
{
    public override ApiResponse<DoctorDto> GetExamples()
    {
        var model = new DoctorDto
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
        };

        return GetResponse(model);
    }
}