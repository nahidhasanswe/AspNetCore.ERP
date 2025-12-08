using ERP.Core.Events;

namespace ERP.Finance.Domain.GeneralLedger.Events;

public record JournalEntryPostedEvent(string Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}