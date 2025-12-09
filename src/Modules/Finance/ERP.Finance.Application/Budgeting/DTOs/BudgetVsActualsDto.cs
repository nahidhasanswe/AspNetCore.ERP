using System;

namespace ERP.Finance.Application.Budgeting.DTOs;

public record BudgetVsActualsDto(
    Guid BudgetId,
    string BudgetName,
    Guid AccountId,
    string AccountName,
    string Period,
    decimal BudgetedAmount,
    decimal CommittedAmount,
    decimal ActualAmount, // From GL
    decimal AvailableAmount, // Budgeted - Committed - Actual
    decimal Variance // Budgeted - Actual
);