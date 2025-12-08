using ERP.Core.Repository;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public interface IAccountRepository : IRepository<Account>
{
    Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken);
    Task<string> GetAccountNameAsync(Guid id, CancellationToken cancellationToken);
    Task<Account?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}