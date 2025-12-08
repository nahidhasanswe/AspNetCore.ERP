using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Domain.Aggregates.DoctorAggregate;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

public class ScheduleRepository(IDbContextProvider<BookingDbContext> dbContextProvider)
    : EfRepository<BookingDbContext, Schedule>(dbContextProvider), IScheduleRepository
{
    public async Task<IEnumerable<Schedule>> GetSchedulesForDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        var dateOnly = date.Date;
        return await Table
            .Where(s => s.DoctorId == doctorId)
            .Where(s => s.IsActive)
            .Where(s => s.DayOfWeek == date.DayOfWeek)
            .Where(s => s.EffectiveFrom.Date <= dateOnly)
            .Where(s => !s.EffectiveTo.HasValue || s.EffectiveTo.Value.Date >= dateOnly)
            .Where(s => s.IsRecurring || s.EffectiveFrom.Date == dateOnly)
            .ToListAsync(cancellationToken);
    }
}
