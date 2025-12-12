using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.DTOs;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.GeneralLedger;

public class GeneralLedgerEntryRepository : EfRepository<FinanceDbContext, GeneralLedgerEntry>, IGeneralLedgerEntryRepository
{
    public GeneralLedgerEntryRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<IReadOnlyCollection<GeneralLedgerEntry>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<AccountBalanceDto>> GetAccountBalancesForPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}