using ERP.Api.Examples.Patient;
using ERP.Core.Swagger.Attributes;
using ERP.Application.DTOs;
using ERP.Core.Mapping;
using ERP.Core.Web.Controller;
using ERP.Core.Web.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.Features.Patient.Commands.RegisterPatiend;
using ERP.Application.Features.Patient.Commands.UpdatePatient;
using ERP.Application.Features.Patient.Queries;

namespace ERP.Api.Api;

[ApiController]
[Route("api/patient")]
public class PatientApiController(ILogger<PatientApiController> logger, IObjectMapper mapper, IMediator mediator) 
    : ApiControllerBase(logger, mapper)
{
    /// <summary>
    /// Get All Patients using filter
    /// </summary>
    /// <param name="search">Optional. Search anything into first name, last name, email, phone number</param>
    /// <param name="gender">Optional. Filter by gender</param>
    /// <param name="city">Optional. Filter by city</param>
    /// <param name="country">Optional. Filter by country</param>
    /// <param name="pageIndex">Optional. Page index for pagination (0 index based). Default is 0</param>
    /// <param name="pageSize">Optional. Page size for pagination. Default value is 50</param>
    /// <param name="sort">Optional. Sorting the result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiReadResponse(typeof(ApiResponse<List<PatientDto>>), typeof(GetPatientsExample))]
    public async Task<IActionResult> GetPatientListAsync(
        [FromQuery] string? search,
        [FromQuery] string? gender,
        [FromQuery] string? city,
        [FromQuery] string? country,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPatientListWithPaginationQuery
        {
            Search = search ?? string.Empty,
            Gender = gender ?? string.Empty,
            City = city ?? string.Empty,
            Country = country ?? string.Empty,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Sort = sort
        };
        
        var response = await mediator.Send(query, cancellationToken);
        return GetResult(response);
    }

    /// <summary>
    /// Get patient information by identifier
    /// </summary>
    /// <param name="id">Patient unique identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ApiReadResponse(typeof(ApiResponse<PatientDto>), typeof(GetPatientExample))]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return GetResultAsNotFoundIfFailed(response);
    }

    /// <summary>
    /// Create new patient
    /// </summary>
    /// <param name="create">Patient information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiWriteResponse(typeof(RegisterPatientCommand), typeof(CreatePatientExample), typeof(ApiResponse<PatientDto>), typeof(GetPatientExample))]
    public async Task<IActionResult> AddPatientAsync([FromBody] RegisterPatientCommand create, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(create, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }

    /// <summary>
    /// Update existing patient information
    /// </summary>
    /// <param name="id">Existing patient identifier</param>
    /// <param name="update">Update patient information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ApiWriteResponse(typeof(UpdatePatientCommand), typeof(UpdatePatientExample), typeof(ApiResponse<PatientDto>), typeof(GetPatientExample))]
    public async Task<IActionResult> UpdatePatientAsync(Guid id, [FromBody] UpdatePatientCommand update,
        CancellationToken cancellationToken = default)
    {
        // Ensure the ID in the route matches the ID in the body
        if (id != update.Id)
        {
            return BadRequest("Patient ID in route and body must match.");
        }

        var response = await mediator.Send(update, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }
}
