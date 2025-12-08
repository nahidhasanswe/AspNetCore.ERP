using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.TaxManagement.Events;

public record TaxCalculatedEvent(
    Guid SourceTransactionId,
    bool IsSalesTransaction,
    Money TaxAmount,
    Guid TaxPayableAccountId,
    Guid SourceControlAccountId,
    string Reference,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}