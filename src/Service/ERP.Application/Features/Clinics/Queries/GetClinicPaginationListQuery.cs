using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Clinics.Queries;

public class GetClinicPaginationListQuery : IPaginationQuery<PaginationResult<ClinicDto>>
{
    public string Search { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool? Active { get; set; } = null;
    
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 50;

    public string? Sort { get; set; } = null;
}