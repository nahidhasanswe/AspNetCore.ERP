using ERP.Core.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IApprovalRuleRepository : IRepository<ApprovalRule>
{
    Task<IEnumerable<ApprovalRule>> GetApplicableRulesAsync(string currency);
}