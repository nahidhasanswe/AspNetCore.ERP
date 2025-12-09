using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.Budgeting.Commands.UpdateBudgetItem;

public class UpdateBudgetItemCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public Guid BudgetItemId { get; set; }
    public Money NewBudgetedAmount { get; set; }
    public string NewPeriod { get; set; }
    public Guid? NewCostCenterId { get; set; }
}