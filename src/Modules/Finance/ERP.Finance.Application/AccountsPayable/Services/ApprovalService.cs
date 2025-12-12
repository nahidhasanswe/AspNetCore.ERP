using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Services;

public class ApprovalService(IApprovalRuleRepository ruleRepository) : IApprovalService
{
    public async Task<bool> IsApprovalRequired(VendorInvoice invoice)
    {
        var rules = await ruleRepository.GetApplicableRulesAsync(invoice.BusinessUnitId);
        return rules.Any(rule => invoice.TotalAmount.Amount > rule.AmountThreshold.Amount);
    }

    public async Task<bool> HasSufficientApproval(VendorInvoice invoice, Guid approverId)
    {
        var rules = await ruleRepository.GetApplicableRulesAsync(invoice.BusinessUnitId);
        var applicableRules = rules.Where(r => invoice.TotalAmount.Amount > r.AmountThreshold.Amount).ToList();

        if (!applicableRules.Any())
        {
            return true; // No special approval needed
        }

        // Find the highest threshold rule that applies
        var mostRestrictiveRule = applicableRules.OrderByDescending(r => r.AmountThreshold.Amount).First();

        // Check if any of the approvers on the invoice match the required approver for the most restrictive rule
        return invoice.ApproverIds.Contains(mostRestrictiveRule.RequiredApproverId);
    }
}