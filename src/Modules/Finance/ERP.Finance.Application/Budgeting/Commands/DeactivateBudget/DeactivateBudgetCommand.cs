using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.DeactivateBudget;

public class DeactivateBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
}