using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record DebitMemoAppliedEvent(
    Guid DebitMemoId,
    Guid VendorId,
    Money Amount,
    DateTime AppliedDate,
    Guid APControlAccountId // The AP Control Account for the vendor
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}