using ERP.Core;
using ERP.Core.Mapping;
using ERP.Domain.Aggregates.DoctorAggregate;
using MediatR;
using ERP.Application.DTOs;

namespace ERP.Application.Features.Doctors.Queries.Schedule;

public class GetScheduleByIdQueryHandler(IScheduleRepository scheduleRepository, IObjectMapper mapper)
    : IRequestHandler<GetScheduleByIdQuery, Result<TimeSlotDto>>
{
    public async Task<Result<TimeSlotDto>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (schedule is null)
        {
            return Result.Failure<TimeSlotDto>("Schedule not found");
        }

        var timeSlotDto = mapper.Map<TimeSlotDto>(schedule);

        return Result.Success(timeSlotDto);
    }
}
