using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.AccountsReceivable;

public class CashReceiptRepository : EfRepository<FinanceDbContext, CashReceipt>, ICashReceiptRepository
{
    public CashReceiptRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}