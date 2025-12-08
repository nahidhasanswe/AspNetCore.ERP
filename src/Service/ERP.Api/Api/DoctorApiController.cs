using ERP.Api.Examples.Doctor;
using ERP.Core.Swagger.Attributes;
using ERP.Application.DTOs;
using ERP.Application.Features.Doctors.Commands.CreateDoctor;
using ERP.Application.Features.Doctors.Commands.UpdateDoctor;
using ERP.Core.Mapping;
using ERP.Core.Web.Controller;
using ERP.Core.Web.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.Features.Doctors.Queries.Doctor;

namespace ERP.Api.Api;

[ApiController]
[Route("api/doctor")]
public class DoctorApiController(ILogger<DoctorApiController> logger, IObjectMapper mapper, IMediator mediator) 
    : ApiControllerBase(logger, mapper)
{
    /// <summary>
    /// Get All Doctors using filter
    /// </summary>
    /// <param name="search">Optional. Search anything into first name, last name, specialization</param>
    /// <param name="specialization">Optional. Filter by specialization</param>
    /// <param name="clinicId">Optional. Filter by clinic ID</param>
    /// <param name="active">Optional. Filter by active status</param>
    /// <param name="pageIndex">Optional. Page index for pagination (0 index based). Default is 0</param>
    /// <param name="pageSize">Optional. Page size for pagination. Default value is 50</param>
    /// <param name="sort">Optional. Sorting the result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiReadResponse(typeof(ApiResponse<List<DoctorDto>>), typeof(GetDoctorsExample))]
    public async Task<IActionResult> GetDoctorListAsync(
        [FromQuery] string? search,
        [FromQuery] string? specialization,
        [FromQuery] Guid? clinicId,
        [FromQuery] bool? active,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDoctorPaginationListQuery
        {
            Search = search ?? string.Empty,
            Specialization = specialization ?? string.Empty,
            ClinicId = clinicId,
            Active = active,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Sort = sort
        };
        
        var response = await mediator.Send(query, cancellationToken);
        return GetResult(response);
    }

    /// <summary>
    /// Get doctor information by identifier
    /// </summary>
    /// <param name="id">Doctor unique identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ApiReadResponse(typeof(ApiResponse<DoctorDto>), typeof(GetDoctorExample))]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new GetDoctorByIdQuery(id), cancellationToken);
        return GetResultAsNotFoundIfFailed(response);
    }

    /// <summary>
    /// Create new doctor
    /// </summary>
    /// <param name="create">Doctor information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiWriteResponse(typeof(CreateDoctorCommand), typeof(CreateDoctorExample), typeof(ApiResponse<DoctorDto>), typeof(GetDoctorExample))]
    public async Task<IActionResult> AddDoctorAsync([FromBody] CreateDoctorCommand create, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(create, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }

    /// <summary>
    /// Update existing doctor information
    /// </summary>
    /// <param name="id">Existing doctor identifier</param>
    /// <param name="update">Update doctor information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ApiWriteResponse(typeof(UpdateDoctorCommand), typeof(UpdateDoctorExample), typeof(ApiResponse<DoctorDto>), typeof(GetDoctorExample))]
    public async Task<IActionResult> UpdateDoctorAsync(Guid id, [FromBody] UpdateDoctorCommand update,
        CancellationToken cancellationToken = default)
    {
        // Ensure the ID in the route matches the ID in the body
        if (id != update.Id)
        {
            return BadRequestResponse("Doctor ID in route and body must match.");
        }

        var response = await mediator.Send(update, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }
}
