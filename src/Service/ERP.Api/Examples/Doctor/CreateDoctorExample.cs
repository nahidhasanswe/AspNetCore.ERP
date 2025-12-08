using ERP.Application.Features.Doctors.Commands.CreateDoctor;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Doctor;

public class CreateDoctorExample : CustomExampleProvider<CreateDoctorCommand>
{
    public override ApiResponse<CreateDoctorCommand> GetExamples()
    {
        var model = new CreateDoctorCommand
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Specialization = "Dermatology",
            LicenseNumber = "DR98765",
            Phone = "555-333-4444",
            Email = "alice.j@example.com",
            ClinicId = Guid.NewGuid() // Assuming a clinic already exists
        };

        return GetResponse(model);
    }
}