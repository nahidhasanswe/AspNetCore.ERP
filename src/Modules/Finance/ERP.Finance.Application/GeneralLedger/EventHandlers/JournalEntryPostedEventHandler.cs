using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

/// <summary>
/// Handles the domain event that is raised when a journal entry is posted.
/// Its primary responsibility is to create the immutable GeneralLedgerEntry records
/// which serve as the financial system's read model for reporting.
/// </summary>
public class JournalEntryPostedEventHandler(
    IUnitOfWorkManager unitOfWork,
    IJournalEntryRepository journalEntryRepository,
    IGeneralLedgerEntryRepository generalLedgerEntryRepository
    ) : INotificationHandler<JournalEntryPostedEvent>
{
    public async Task Handle(JournalEntryPostedEvent notification, CancellationToken cancellationToken)
    {
        var journalEntryId = Guid.Parse(notification.Id);

        // 1. Retrieve the full aggregate that was just posted.
        var journalEntry = await journalEntryRepository.GetByIdAsync(journalEntryId, cancellationToken);

        if (journalEntry is null || !journalEntry.IsPosted)
        {
            // This should not happen in a consistent system, but it's good practice to guard against it.
            // Consider logging this anomaly.
            return;
        }

        // 2. Create a GeneralLedgerEntry for each line in the journal entry.
        foreach (var line in journalEntry.Lines)
        {
            var ledgerEntry = new GeneralLedgerEntry(line, journalEntry.PostingDate, journalEntry.Id);
            await generalLedgerEntryRepository.AddAsync(ledgerEntry, cancellationToken);
        }

        // 3. Persist the new read-model entries.
        // Note: This handler operates in its own transaction scope, separate from the command that triggered it.
        using var scope = unitOfWork.Begin();
        await scope.SaveChangesAsync(cancellationToken);
    }
}