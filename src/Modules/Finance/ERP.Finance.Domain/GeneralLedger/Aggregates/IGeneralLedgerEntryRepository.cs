using ERP.Core.Repository;
using ERP.Finance.Domain.GeneralLedger.DTOs;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public interface IGeneralLedgerEntryRepository : IRepository<GeneralLedgerEntry>
{
    Task<IReadOnlyCollection<GeneralLedgerEntry>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AccountBalanceDto>> GetAccountBalancesForPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}