using ERP.Core.Repository;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public interface IAccountRepository : IRepository<Account>
{
    Task<string> GetAccountNameAsync(Guid id, CancellationToken cancellationToken);
}