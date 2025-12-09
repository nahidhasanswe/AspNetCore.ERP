using ERP.Core;
using MediatR;
namespace ERP.Finance.Application.Budgeting.Commands.SubmitBudgetForApproval;

public class SubmitBudgetForApprovalCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
}