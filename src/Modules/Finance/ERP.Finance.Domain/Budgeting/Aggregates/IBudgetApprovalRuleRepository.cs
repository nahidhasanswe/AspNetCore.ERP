using ERP.Core.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public interface IBudgetApprovalRuleRepository : IRepository<BudgetApprovalRule>
{
    Task<IEnumerable<BudgetApprovalRule>> GetApplicableRulesAsync(Guid businessUnitId, string currency);
}