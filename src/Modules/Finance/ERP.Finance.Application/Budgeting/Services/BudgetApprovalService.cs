using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Finance.Application.Budgeting.Services;

public class BudgetApprovalService : IBudgetApprovalService
{
    private readonly IBudgetApprovalRuleRepository _ruleRepository;

    public BudgetApprovalService(IBudgetApprovalRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    public async Task<bool> IsApprovalRequired(Budget budget)
    {
        var rules = await _ruleRepository.GetApplicableRulesAsync(budget.BusinessUnitId, budget.Items.FirstOrDefault()?.BudgetedAmount.Currency ?? "USD");
        return rules.Any(rule => budget.Items.Sum(item => item.BudgetedAmount.Amount) > rule.AmountThreshold.Amount);
    }

    public async Task<bool> HasSufficientApproval(Budget budget, Guid approverId)
    {
        var rules = await _ruleRepository.GetApplicableRulesAsync(budget.BusinessUnitId, budget.Items.FirstOrDefault()?.BudgetedAmount.Currency ?? "USD");
        var applicableRules = rules.Where(r => budget.Items.Sum(item => item.BudgetedAmount.Amount) > r.AmountThreshold.Amount).ToList();

        if (!applicableRules.Any())
        {
            return true; // No special approval needed
        }

        // Find the highest threshold rule that applies
        var mostRestrictiveRule = applicableRules.OrderByDescending(r => r.AmountThreshold.Amount).FirstOrDefault();

        // Check if any of the approvers on the budget match the required approver for the most restrictive rule
        return budget.ApproverIds.Contains(mostRestrictiveRule.RequiredApproverId);
    }
}