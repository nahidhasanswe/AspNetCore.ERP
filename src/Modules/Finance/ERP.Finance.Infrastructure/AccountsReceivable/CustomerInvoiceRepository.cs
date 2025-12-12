using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.AccountsReceivable;

public class CustomerInvoiceRepository : EfRepository<FinanceDbContext, CustomerInvoice>, ICustomerInvoiceRepository
{
    public CustomerInvoiceRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}