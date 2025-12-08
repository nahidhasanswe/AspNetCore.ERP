using ERP.Application.DTOs;
using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Application.Features.Doctors.Queries.Doctor;

public class GetDoctorPaginationListQuery : IPaginationQuery<PaginationResult<DoctorDto>>
{
    public string? Search { get; set; }
    public string? Specialization { get; set; }
    public Guid? ClinicId { get; set; }
    public bool? Active { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 50;
    public string? Sort { get; set; }
}