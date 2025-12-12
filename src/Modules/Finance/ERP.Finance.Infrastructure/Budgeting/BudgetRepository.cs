using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.Budgeting;

public class BudgetRepository : EfRepository<FinanceDbContext, Budget>, IBudgetRepository
{
    public BudgetRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<Budget> GetApprovedBudgetForYear(int fiscalYear)
    {
        throw new NotImplementedException();
    }

    public Task<Budget?> GetActiveBudgetForAccountAsync(Guid glAccountId, Guid? costCenterId)
    {
        throw new NotImplementedException();
    }
}