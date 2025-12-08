using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public interface ICustomerCreditProfileRepository : IRepository<CustomerCreditProfile>
{
    Task<CustomerCreditProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}