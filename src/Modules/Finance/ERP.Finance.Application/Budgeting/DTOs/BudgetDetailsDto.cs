using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.Budgeting.DTOs;

public record BudgetDetailsDto(
    Guid Id,
    Guid BusinessUnitId,
    string Name,
    string FiscalPeriod,
    DateTime StartDate,
    DateTime EndDate,
    BudgetStatus Status,
    IReadOnlyCollection<BudgetItemDetailsDto> Items
);

public record BudgetItemDetailsDto(
    Guid Id,
    Guid AccountId,
    string AccountName, // Will be populated by handler
    Money BudgetedAmount,
    Money CommittedAmount,
    string Period,
    Guid? CostCenterId
);