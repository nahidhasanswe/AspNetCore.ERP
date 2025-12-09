using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<string?> GetNameByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}