using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.GeneralLedger;

public class JournalEntryRepository : EfRepository<FinanceDbContext, JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}