using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.ApproveBudget;

public class ApproveBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public Guid ApproverId { get; set; } // Who approved it
}