using ERP.Finance.Domain.Budgeting.DTOs;

namespace ERP.Finance.Domain.Budgeting.Service;

public interface IBudgetComparisonService
{
    // Returns a DTO containing Budgets, Actuals, and Variances
    Task<List<BudgetComparisonResult>> CompareBudgetToActuals(Guid budgetId, string period, CancellationToken cancellationToken);
}