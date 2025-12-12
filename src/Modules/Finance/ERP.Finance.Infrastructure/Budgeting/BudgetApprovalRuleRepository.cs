using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.Budgeting;

public class BudgetApprovalRuleRepository : EfRepository<FinanceDbContext, BudgetApprovalRule>, IBudgetApprovalRuleRepository
{
    public BudgetApprovalRuleRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<IEnumerable<BudgetApprovalRule>> GetApplicableRulesAsync(Guid businessUnitId, string currency)
    {
        throw new NotImplementedException();
    }
}