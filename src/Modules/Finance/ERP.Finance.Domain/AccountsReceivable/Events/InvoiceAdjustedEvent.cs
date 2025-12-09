using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record InvoiceAdjustedEvent(
    Guid InvoiceId,
    Money AdjustmentAmount,
    string Reason,
    Guid ARControlAccountId,
    Guid AdjustmentAccountId, // The GL account for the adjustment (e.g., Sales Returns, Discount Expense)
    DateTime AdjustmentDate,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}