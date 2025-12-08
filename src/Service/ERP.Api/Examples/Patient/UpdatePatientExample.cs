using ERP.Core.Web.Response;
using System;
using ERP.Application.Features.Patient.Commands.UpdatePatient;

namespace ERP.Api.Examples.Patient;

public class UpdatePatientExample : CustomExampleProvider<UpdatePatientCommand>
{
    public override ApiResponse<UpdatePatientCommand> GetExamples()
    {
        var model = new UpdatePatientCommand
        {
            Id = Guid.NewGuid(), // Example Patient Id
            FirstName = "Alice",
            LastName = "Smith-Updated",
            DateOfBirth = new DateTime(1995, 8, 20),
            Gender = "Female",
            Email = "alice.smith.updated@example.com",
            Phone = "555-111-3333",
            Street = "123 Maple Ave",
            City = "Springfield",
            Country = "USA",
            EmergencyContact = "Bob Smith (Brother) - 555-222-4444"
        };

        return GetResponse(model);
    }
}