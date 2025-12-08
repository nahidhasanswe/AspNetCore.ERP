using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;
using System.Collections.Generic;

namespace ERP.Api.Examples.Patient;

public class GetPatientsExample : CustomExampleProvider<List<PatientDto>>
{
    public override ApiResponse<List<PatientDto>> GetExamples()
    {
        var patients = new List<PatientDto>
        {
            new PatientDto
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
            },
            new PatientDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Michael",
                LastName = "Green",
                DateOfBirth = new DateTime(1985, 11, 20),
                Gender = "Male",
                Email = "michael.g@example.com",
                PhoneNumber = "555-666-7777",
                Address = "22B Baker St",
                City = "London",
                Country = "UK",
                EmergencyContact = "Sarah Green (Wife) - 555-888-9999"
            }
        };

        return GetResponse(patients);
    }
}