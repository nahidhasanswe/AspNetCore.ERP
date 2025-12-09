using ERP.Core;
using ERP.Finance.Application.FiscalYear.DTOs;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;
namespace ERP.Finance.Application.FiscalYear.Queries.ListFiscalPeriods;

public class ListFiscalPeriodsQueryHandler(IFiscalPeriodRepository fiscalPeriodRepository)
    : IRequestHandler<ListFiscalPeriodsQuery, Result<IEnumerable<FiscalPeriodDto>>>
{
    public async Task<Result<IEnumerable<FiscalPeriodDto>>> Handle(ListFiscalPeriodsQuery request, CancellationToken cancellationToken)
    {
        var allPeriods = await fiscalPeriodRepository.ListAllAsync(cancellationToken);

        var filteredPeriods = allPeriods.AsQueryable();

        if (request.Year.HasValue)
        {
            filteredPeriods = filteredPeriods.Where(p => p.StartDate.Year == request.Year.Value);
        }

        if (request.Status.HasValue)
        {
            filteredPeriods = filteredPeriods.Where(p => p.Status == request.Status.Value);
        }

        var dtos = filteredPeriods.Select(p => new FiscalPeriodDto(
            p.Id,
            p.Name,
            p.StartDate,
            p.EndDate,
            p.Status.ToString()
        )).ToList();

        return Result.Success(dtos.AsEnumerable());
    }
}