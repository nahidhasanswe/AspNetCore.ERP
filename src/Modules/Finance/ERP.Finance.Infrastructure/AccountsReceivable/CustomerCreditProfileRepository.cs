using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.AccountsReceivable;

public class CustomerCreditProfileRepository : EfRepository<FinanceDbContext, CustomerCreditProfile>, ICustomerCreditProfileRepository
{
    public CustomerCreditProfileRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<CustomerCreditProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}