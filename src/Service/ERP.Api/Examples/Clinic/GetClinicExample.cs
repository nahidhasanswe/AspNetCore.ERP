using ERP.Application.DTOs;
using ERP.Core.Web.Response;
using System;

namespace ERP.Api.Examples.Clinic;

public class GetClinicExample : CustomExampleProvider<ClinicDto>
{
    public override ApiResponse<ClinicDto> GetExamples()
    {
        var model = new ClinicDto
        {
            Id = Guid.NewGuid(),
            Name = "Example Clinic",
            Address = "789 Pine St",
            Phone = "555-111-2222",
            Email = "info@exampleclinic.com",
            City = "Sampleton",
            Country = "USA",
            IsActive = true
        };

        return GetResponse(model);
    }
}