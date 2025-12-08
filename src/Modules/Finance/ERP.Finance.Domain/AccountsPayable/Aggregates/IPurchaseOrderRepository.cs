using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    // Add any specific query methods needed for POs in the future
}