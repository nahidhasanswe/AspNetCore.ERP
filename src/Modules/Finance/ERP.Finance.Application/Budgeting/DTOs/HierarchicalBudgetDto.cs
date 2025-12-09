using ERP.Finance.Domain.Shared.Enums;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.Budgeting.DTOs;

public record HierarchicalBudgetDto(
    Guid Id,
    string Name,
    string FiscalPeriod,
    BudgetStatus Status,
    Guid BusinessUnitId,
    Guid? ParentBudgetId,
    List<HierarchicalBudgetDto> Children
);