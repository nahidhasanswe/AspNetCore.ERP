using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Patient;

public class GetPatientExample : CustomExampleProvider<PatientDto>
{
    public override ApiResponse<PatientDto> GetExamples()
    {
        var model = new PatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Emily",
            LastName = "Brown",
            DateOfBirth = new DateTime(1990, 5, 15),
            Gender = "Female",
            Email = "emily.b@example.com",
            PhoneNumber = "555-444-5555",
            Address = "10 Downing St",
            City = "London",
            Country = "UK",
            EmergencyContact = "John Brown (Husband) - 555-111-2222"
        };

        return GetResponse(model);
    }
}