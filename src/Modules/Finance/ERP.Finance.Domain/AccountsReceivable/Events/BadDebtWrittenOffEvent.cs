using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record BadDebtWrittenOffEvent(
    Guid InvoiceId,
    Money WriteOffAmount,
    DateTime WriteOffDate,
    Guid ARControlAccountId,        // The account to be Credited (Asset decrease)
    Guid BadDebtExpenseAccountId,   // The account to be Debited (Expense increase)
    string Reason,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}