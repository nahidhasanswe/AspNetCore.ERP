using ERP.Finance.Domain.Budgeting.Aggregates;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.Budgeting.Service;

public interface IBudgetApprovalService
{
    Task<bool> IsApprovalRequired(Budget budget);
    Task<bool> HasSufficientApproval(Budget budget, Guid approverId);
}