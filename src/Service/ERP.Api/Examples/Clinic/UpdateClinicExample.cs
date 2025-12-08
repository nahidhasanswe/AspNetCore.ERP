using ERP.Application.Features.Clinics.Commands.UpdateClinic;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Clinic;

public class UpdateClinicExample : CustomExampleProvider<UpdateClinicCommand>
{
    public override ApiResponse<UpdateClinicCommand> GetExamples()
    {
        var model = new UpdateClinicCommand
        {
            Id = Guid.NewGuid(), // Example Id
            Name = "Updated Central Clinic",
            Street = "456 Oak Ave",
            City = "Metropolis",
            State = "IL",
            Country = "USA",
            PostalCode = "54321",
            Phone = "555-987-6543",
            Email = "update@centralclinic.com"
        };

        return GetResponse(model);
    }
}