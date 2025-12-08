using ERP.Core.Repository;

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<Budget> GetApprovedBudgetForYear(int fiscalYear);
    Task<Budget?> GetActiveBudgetForAccountAsync(Guid glAccountId, Guid? costCenterId);
}