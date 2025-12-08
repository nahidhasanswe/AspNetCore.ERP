using ERP.Application.Features.Clinics.Commands.CreateClinic;
using ERP.Core.Web.Response;

namespace ERP.Api.Examples.Clinic;

public class CreateClinicExample : CustomExampleProvider<CreateClinicCommand>
{
    public override ApiResponse<CreateClinicCommand> GetExamples()
    {
        var model = new CreateClinicCommand
        {
            Name = "Central Clinic",
            Street = "123 Main St",
            City = "Metropolis",
            State = "IL",
            Country = "USA",
            PostalCode = "12345",
            Phone = "555-123-4567",
            Email = "contact@centralclinic.com"
        };

        return GetResponse(model);
    }
}