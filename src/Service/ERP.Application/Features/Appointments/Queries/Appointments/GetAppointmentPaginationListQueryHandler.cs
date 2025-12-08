using ERP.Application.DTOs;
using ERP.Core;
using MediatR;
using ERP.Core.Mapping;
using ERP.Domain.Specifications.Appointment;
using ERP.Core.Collections;
using ERP.Domain.Aggregates.AppointmentAggregate;

namespace ERP.Application.Features.Appointments.Queries.Appointments;

public class GetAppointmentPaginationListQueryHandler(
    IAppointmentRepository appointmentRepository,
    IObjectMapper mapper)
    : IRequestHandler<GetAppointmentPaginationListQuery, PaginationResult<AppointmentDto>>
{
    public async Task<PaginationResult<AppointmentDto>> Handle(GetAppointmentPaginationListQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAppointmentPaginationListSpecification(
            request.PatientId,
            request.DoctorId,
            request.ClinicId,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.PageIndex,
            request.PageSize,
            request.Sort
        );

        var response = await appointmentRepository.GetPaginationListAsync(spec, cancellationToken);
        return Result.SuccessForPagination<AppointmentDto>(mapper.Map<IPagedList<AppointmentDto>>(response));
    }
}
