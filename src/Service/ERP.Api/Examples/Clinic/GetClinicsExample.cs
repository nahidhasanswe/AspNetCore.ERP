using ERP.Application.DTOs;
using ERP.Core.Web.Response;

namespace ERP.Api.Examples.Clinic;

public class GetClinicsExample : CustomExampleProvider<List<ClinicDto>>
{
    public override ApiResponse<List<ClinicDto>> GetExamples()
    {
        var clinics = new List<ClinicDto>
        {
            new ClinicDto
            {
                Id = Guid.NewGuid(),
                Name = "First City Clinic",
                Address = "101 Main St",
                Phone = "555-111-1111",
                Email = "info@firstcityclinic.com",
                City = "Metropolis",
                Country = "USA",
                IsActive = true
            },
            new ClinicDto
            {
                Id = Guid.NewGuid(),
                Name = "Second Town Health",
                Address = "202 Oak Ave",
                Phone = "555-222-2222",
                Email = "contact@secondtownhealth.com",
                City = "Smallville",
                Country = "USA",
                IsActive = true
            },
            new ClinicDto
            {
                Id = Guid.NewGuid(),
                Name = "Community Care Center",
                Address = "303 Elm Rd",
                Phone = "555-333-3333",
                Email = "admin@communitycare.com",
                City = "Gotham",
                Country = "USA",
                IsActive = false
            }
        };

        return GetResponse(clinics);
    }
}