using ERP.Core;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.FiscalYear.Enums;
using ERP.Finance.Domain.FiscalYear.Service;

namespace ERP.Finance.Application.FiscalYear.Services;

public class FiscalPeriodCheckService(IFiscalPeriodRepository repository) : IFiscalPeriodCheckService
{
    // New repository

    public async Task<Result<bool>> EnsurePeriodIsOpenForPosting(DateTime transactionDate)
    {
        var period = await repository.GetPeriodByDateAsync(transactionDate.Date, CancellationToken.None);

        if (period == null)
            throw new DomainException($"No fiscal period defined for date {transactionDate.ToShortDateString()}.");

        if (period.Status == PeriodStatus.HardClose)
            throw new DomainException($"Posting is forbidden. Period {period.Name} is permanently closed.");

        if (period.Status == PeriodStatus.SoftClose)
            // You could allow posting only if the user has a special 'Adjustment' role here.
            throw new DomainException($"Posting is restricted. Period {period.Name} is soft-closed."); 
            
        // Status is Open: Success

        return Result.Success(true);
    }
}