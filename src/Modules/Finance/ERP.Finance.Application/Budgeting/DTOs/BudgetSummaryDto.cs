using ERP.Finance.Domain.Shared.Enums;
using System;

namespace ERP.Finance.Application.Budgeting.DTOs;

public record BudgetSummaryDto(
    Guid Id,
    string Name,
    string FiscalPeriod,
    DateTime StartDate,
    DateTime EndDate,
    BudgetStatus Status,
    Guid BusinessUnitId
);