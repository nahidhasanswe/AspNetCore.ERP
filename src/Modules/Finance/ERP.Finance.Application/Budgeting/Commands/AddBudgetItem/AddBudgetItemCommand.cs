using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.Budgeting.Commands.AddBudgetItem;

public class AddBudgetItemCommand : IRequestCommand<Guid>
{
    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; } // GL Account ID
    public Money BudgetedAmount { get; set; }
    public string Period { get; set; } // e.g., "JAN-2026", "Q1-2026"
    public Guid? CostCenterId { get; set; }
}