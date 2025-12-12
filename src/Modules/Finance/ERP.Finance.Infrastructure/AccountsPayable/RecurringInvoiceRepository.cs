using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class RecurringInvoiceRepository(IDbContextProvider<FinanceDbContext> dbContextProvider)
    : EfRepository<FinanceDbContext, RecurringInvoice>(dbContextProvider), IRecurringInvoiceRepository
{
    public async Task<IEnumerable<RecurringInvoice>> GetActiveRecurringInvoicesDueForGenerationAsync(DateTime generationDate)
    {
        return await Table.Where(x => x.EndDate <= generationDate).ToListAsync();
    }
}