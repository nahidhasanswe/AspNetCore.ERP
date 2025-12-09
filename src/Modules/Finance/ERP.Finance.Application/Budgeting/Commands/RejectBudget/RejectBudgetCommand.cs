using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.RejectBudget;

public class RejectBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
}