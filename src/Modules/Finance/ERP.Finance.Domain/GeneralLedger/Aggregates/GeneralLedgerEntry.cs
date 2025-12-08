using ERP.Core.Entities;
using ERP.Finance.Domain.GeneralLedger.Aggregates;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

/// <summary>
/// Represents a single, immutable entry in the general ledger.
/// This is a "read model" entity, populated when a Journal Entry is posted.
/// It is optimized for querying and reporting.
/// </summary>
public class GeneralLedgerEntry : Entity
{
    public Guid AccountId { get; private set; }
    public DateTime PostingDate { get; private set; }
    public Guid JournalEntryId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string Currency { get; private set; }
    public string Description { get; private set; }
    public Guid? CostCenterId { get; private set; }

    private GeneralLedgerEntry() { }

    public GeneralLedgerEntry(LedgerLine line, DateTime postingDate, Guid journalEntryId) : base(Guid.NewGuid())
    {
        AccountId = line.AccountId;
        PostingDate = postingDate;
        JournalEntryId = journalEntryId;
        Debit = line.IsDebit ? line.Amount.Amount : 0;
        Credit = !line.IsDebit ? line.Amount.Amount : 0;
        Currency = line.Amount.Currency;
        Description = line.Description;
        CostCenterId = line.CostCenterId;
    }
}