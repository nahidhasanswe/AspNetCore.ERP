using ERP.Core.Events;

namespace ERP.Finance.Domain.GeneralLedger.Events;

public class JournalEntryPostedEvent(string id) : IDomainEvent
{
    public string Id { get; set; } = id;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}