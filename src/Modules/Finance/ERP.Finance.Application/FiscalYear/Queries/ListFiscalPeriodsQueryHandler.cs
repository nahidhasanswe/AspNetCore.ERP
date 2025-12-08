using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.FiscalYear.DTOs;
using ERP.Finance.Domain.FiscalYear.Aggregates;

namespace ERP.Finance.Application.FiscalYear.Queries;

public class ListFiscalPeriodsQueryHandler(IFiscalPeriodRepository repository)
    : IRequestCommandHandler<ListFiscalPeriodsQuery, IEnumerable<FiscalPeriodDto>>
{
    public async Task<Result<IEnumerable<FiscalPeriodDto>>> Handle(ListFiscalPeriodsQuery query, CancellationToken cancellationToken)
    {
        // In a true CQRS system, this would bypass the repository and go to a Read Model/Query Service.
        var periods = await repository.ListAllAsync(); 
        
        // Apply basic filtering
        if (query.YearFilter.HasValue)
        {
            periods = periods.Where(p => p.StartDate.Year == query.YearFilter.Value);
        }

        // Map to DTO
        var result =  periods.Select(p => new FiscalPeriodDto
        (
            p.Id,
            p.Name,
            p.StartDate,
            p.EndDate,
            p.Status.ToString()
        )).OrderByDescending(p => p.StartDate).AsEnumerable();

        return Result.Success(result);
    }
}