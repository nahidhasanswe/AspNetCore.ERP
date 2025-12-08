using ERP.Application.Features.Doctors.Commands.UpdateDoctor;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Doctor;

public class UpdateDoctorExample : CustomExampleProvider<UpdateDoctorCommand>
{
    public override ApiResponse<UpdateDoctorCommand> GetExamples()
    {
        var model = new UpdateDoctorCommand
        {
            Id = Guid.NewGuid(), // Example Doctor Id
            FirstName = "Alice",
            LastName = "Johnson-Updated",
            Specialization = "Dermatology",
            LicenseNumber = "DR98765",
            Phone = "555-333-4444",
            Email = "alice.j.updated@example.com",
            ClinicId = Guid.NewGuid() // Assuming a clinic already exists
        };

        return GetResponse(model);
    }
}