using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.FiscalYear.Enums;
using ERP.Finance.Domain.GeneralLedger.Services;

namespace ERP.Finance.Application.GeneralLedger.Services;

public class GlPeriodStatusService(IFiscalPeriodRepository fiscalPeriodRepository) : IGlPeriodStatusService
{
    /// <summary>
    /// Checks if a given date falls within a fiscal period that is currently open for posting.
    /// </summary>
    public async Task<bool> IsPeriodOpenForPosting(DateTime date)
    {
        var period = await fiscalPeriodRepository.GetPeriodByDateAsync(date);
        if (period == null)
        {
            return false; // No period found for this date
        }

        return period.Status == PeriodStatus.Open;
    }
}