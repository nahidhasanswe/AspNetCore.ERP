using System;

namespace ERP.Finance.Application.Budgeting.DTOs;

public record BudgetAvailabilityDto(
    Guid BudgetId,
    string BudgetName,
    Guid AccountId,
    string AccountName,
    string Period,
    decimal BudgetedAmount,
    decimal CommittedAmount,
    decimal AvailableToCommitAmount, // Budgeted - Committed
    decimal AvailableToSpendAmount // Budgeted - Committed - Actual (from GL)
);