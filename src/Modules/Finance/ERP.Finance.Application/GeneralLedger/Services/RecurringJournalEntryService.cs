using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;

namespace ERP.Finance.Application.GeneralLedger.Services;

public class RecurringJournalEntryService(
    IRecurringJournalEntryRepository recurringJournalEntryRepository,
    IJournalEntryRepository journalEntryRepository,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidationService,
    IUnitOfWorkManager unitOfWorkManager)
{
    public async Task ProcessRecurringJournalEntries(Guid businessUnitId, DateTime processingDate)
    {
        // 1. Get the fiscal period for the processing date. If none is open, we can't proceed.
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(processingDate, CancellationToken.None);
        if (fiscalPeriod is null || fiscalPeriod.Status != Domain.FiscalYear.Enums.PeriodStatus.Open)
        {
            // Log that processing is skipped because no open fiscal period was found.
            return;
        }

        // 2. Get all active recurring journal entries
        var recurringEntries = await recurringJournalEntryRepository.GetAllActiveAsync(processingDate, CancellationToken.None);

        // 3. Filter entries that are due for posting
        var dueEntries = recurringEntries.Where(e => IsDueForPosting(e, processingDate)).ToList();

        if (!dueEntries.Any()) return;

        using var scope = unitOfWorkManager.Begin();

        foreach (var recurringEntry in dueEntries)
        {
            // 4. Create a new JournalEntry from the recurring template
            var journalEntry = recurringEntry.CreateJournalEntry();

            // Re-create ledger lines for the new journal entry instance
            foreach (var recurringLine in recurringEntry.Lines)
            {
                // Note: This assumes the Amount and BaseAmount on the recurring line are the desired values.
                // In a multi-currency system, BaseAmount might need recalculation here.
                var newLine = new LedgerLine(
                    businessUnitId,
                    journalEntry.Id,
                    recurringLine.AccountId,
                    recurringLine.Amount,
                    recurringLine.BaseAmount,
                    recurringLine.IsDebit,
                    recurringLine.Description,
                    recurringLine.CostCenterId
                );
                journalEntry.AddLine(newLine);
            }

            // 5. Post the new JournalEntry
            journalEntry.Post(fiscalPeriod, accountValidationService);
            await journalEntryRepository.AddAsync(journalEntry, CancellationToken.None);

            // 6. Update the LastPostedDate on the recurring entry to prevent re-processing
            recurringEntry.UpdateLastPostedDate(processingDate);
            await recurringJournalEntryRepository.UpdateAsync(recurringEntry, CancellationToken.None);
        }

        // 7. Persist all changes
        await scope.SaveChangesAsync(CancellationToken.None);
    }

    private bool IsDueForPosting(RecurringJournalEntry entry, DateTime processingDate)
    {
        // Basic monthly frequency check. This can be expanded for other frequencies.
        if (entry.Frequency.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
        {
            // True if it has never been posted, or if the processing date is in a subsequent month.
            return entry.LastPostedDate == default || (processingDate.Year > entry.LastPostedDate.Year || (processingDate.Year == entry.LastPostedDate.Year && processingDate.Month > entry.LastPostedDate.Month));
        }

        // Add logic for "Quarterly", "Annually", etc. here
        return false;
    }
}