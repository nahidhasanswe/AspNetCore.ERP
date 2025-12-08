using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface ICreditMemoRepository : IRepository<CreditMemo>
{
    // Add any specific query methods needed for Credit Memos in the future
}