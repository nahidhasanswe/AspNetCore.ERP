using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.TaxManagement.Events;

public record TaxCalculatedEvent(
    Guid TaxTransactionId, // Unique ID for this tax calculation instance
    Guid SourceTransactionId, // e.g., Invoice ID
    Money BaseAmount,
    Money TaxAmount,
    Guid JurisdictionId,
    string JurisdictionCode,
    DateTime TransactionDate,
    Guid TaxPayableAccountId,
    Guid SourceControlAccountId,
    bool IsSalesTransaction, 
    Guid? CostCenterId,
    string Reference,
    Guid BusinessUnitId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}