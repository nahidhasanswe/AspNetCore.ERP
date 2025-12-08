using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}