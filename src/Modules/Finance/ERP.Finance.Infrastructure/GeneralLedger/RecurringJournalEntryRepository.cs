using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.GeneralLedger;

public class RecurringJournalEntryRepository : EfRepository<FinanceDbContext, RecurringJournalEntry>, IRecurringJournalEntryRepository
{
    public RecurringJournalEntryRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<IReadOnlyCollection<RecurringJournalEntry>> GetAllActiveAsync(DateTime processingDate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}