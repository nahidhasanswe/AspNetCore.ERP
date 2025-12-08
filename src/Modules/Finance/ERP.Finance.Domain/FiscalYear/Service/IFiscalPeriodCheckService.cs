using ERP.Core;

namespace ERP.Finance.Domain.FiscalYear.Service;

public interface IFiscalPeriodCheckService
{
    Task<Result<bool>> EnsurePeriodIsOpenForPosting(DateTime transactionDate);
}