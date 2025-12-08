using ERP.Core.Repository;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public interface IRecurringJournalEntryRepository : IRepository<RecurringJournalEntry>
{
    Task<IReadOnlyCollection<RecurringJournalEntry>> GetAllActiveAsync(DateTime processingDate,
        CancellationToken cancellationToken);
}