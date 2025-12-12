using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.GeneralLedger;

public class AccountRepository : EfRepository<FinanceDbContext, Account>, IAccountRepository
{
    public AccountRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetAccountNameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}