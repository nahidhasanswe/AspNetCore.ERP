using ERP.Api.Examples.Clinic;
using ERP.Core.Swagger.Attributes;
using ERP.Application.DTOs;
using ERP.Application.Features.Clinics.Commands.CreateClinic;
using ERP.Application.Features.Clinics.Commands.UpdateClinic;
using ERP.Application.Features.Clinics.Queries;
using ERP.Core.Mapping;
using ERP.Core.Web.Controller;
using ERP.Core.Web.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Api;

[ApiController]
[Route("api/clinic")]
public class ClinicApiController(ILogger<ClinicApiController> logger, IObjectMapper mapper, IMediator mediator) 
    : ApiControllerBase(logger, mapper)
{
    /// <summary>
    /// Get All Clinics using filter
    /// </summary>
    /// <param name="search">Optional. Search anything into name, address</param>
    /// <param name="city">Optional. Filter by city</param>
    /// <param name="country">Optional. Filter by country</param>
    /// <param name="active">Optional. Filter by active status</param>
    /// <param name="pageIndex">Optional. Page index for pagination (0 index based). Default is 0</param>
    /// <param name="pageSize">Optional. Page size for pagination. Default value is 50</param>
    /// <param name="sort">Optional. Sorting the result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiReadResponse(typeof(ApiResponse<ClinicDto[]>), typeof(GetClinicsExample))]
    public async Task<IActionResult> GetClinicListAsync(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? country,
        [FromQuery] bool? active,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetClinicPaginationListQuery
        {
            Search = search ?? string.Empty,
            City = city ?? string.Empty,
            Country = country ?? string.Empty,
            Active = active,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Sort = sort
        };
        
        var response = await mediator.Send(query, cancellationToken);
        return GetResult(response);
    }

    /// <summary>
    /// Get clinic information by identifier
    /// </summary>
    /// <param name="id">Clinic unique identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ApiReadResponse(typeof(ApiResponse<ClinicDto>), typeof(GetClinicExample))]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new GetClinicByIdQuery(id), cancellationToken);
        return GetResultAsNotFoundIfFailed(response);
    }

    /// <summary>
    /// Create new clinic
    /// </summary>
    /// <param name="create">Clinic information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiWriteResponse(typeof(CreateClinicCommand), typeof(CreateClinicExample), typeof(ApiResponse<ClinicDto>), typeof(GetClinicExample))]
    public async Task<IActionResult> AddClinicAsync([FromBody] CreateClinicCommand create, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(create, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }

    /// <summary>
    /// Update existing clinic information
    /// </summary>
    /// <param name="id">Existing clinic identifier</param>
    /// <param name="update">Update clinic information</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ApiWriteResponse(typeof(UpdateClinicCommand), typeof(UpdateClinicExample), typeof(ApiResponse<ClinicDto>), typeof(GetClinicExample))]
    public async Task<IActionResult> UpdateClinicAsync(Guid id, [FromBody] UpdateClinicCommand update,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(update, cancellationToken);
        return await GetResultAsAction(response,
            async () => await GetByIdAsync(response.Value, cancellationToken));
    }
}