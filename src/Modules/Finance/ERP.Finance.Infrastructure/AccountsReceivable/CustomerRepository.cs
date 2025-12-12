using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.AccountsReceivable;

public class CustomerRepository : EfRepository<FinanceDbContext, Customer>, ICustomerRepository
{
    public CustomerRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<string?> GetNameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}