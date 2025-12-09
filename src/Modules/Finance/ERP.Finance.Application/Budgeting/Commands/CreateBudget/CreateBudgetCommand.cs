using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudget;

public class CreateBudgetCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; }
    public string Name { get; set; }
    public string FiscalPeriod { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ParentBudgetId { get; set; } // Added for hierarchy
    public List<BudgetItemDto> Items { get; set; } = new();

    public class BudgetItemDto
    {
        public Guid AccountId { get; set; } // GL Account ID
        public Money BudgetedAmount { get; set; }
        public string Period { get; set; } // e.g., "JAN-2026", "Q1-2026"
        public Guid? CostCenterId { get; set; }
    }
}