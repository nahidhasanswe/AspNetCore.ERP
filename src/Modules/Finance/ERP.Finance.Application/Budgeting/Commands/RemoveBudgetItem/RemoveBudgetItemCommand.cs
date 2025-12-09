using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.RemoveBudgetItem;

public class RemoveBudgetItemCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public Guid BudgetItemId { get; set; }
}