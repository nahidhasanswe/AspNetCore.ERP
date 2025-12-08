using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record DeductionAppliedToInvoiceEvent(
    Guid InvoiceId,
    Money DeductionAmount,
    string DeductionReasonCode,
    Guid ARControlAccountId, // The AR account to be credited
    Guid DeductionExpenseAccountId // The Expense/Discount account to be debited
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}