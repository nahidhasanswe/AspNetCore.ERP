using ERP.Core.Aggregates;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class RecurringJournalEntry : AggregateRoot
{
    public string Description { get; private set; }
    public string ReferenceNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Frequency { get; private set; } // e.g., "Monthly", "Quarterly"
    public DateTime LastPostedDate { get; private set; }

    private readonly List<LedgerLine> _lines = new();
    public IReadOnlyCollection<LedgerLine> Lines => _lines.AsReadOnly();

    private RecurringJournalEntry() { }

    public RecurringJournalEntry(string description, string referenceNumber, DateTime startDate, DateTime endDate, string frequency) : base(Guid.NewGuid())
    {
        Description = description;
        ReferenceNumber = referenceNumber;
        StartDate = startDate;
        EndDate = endDate;
        Frequency = frequency;
        LastPostedDate = default;
    }

    public void AddLine(LedgerLine line)
    {
        _lines.Add(line);
    }

    public void UpdateLastPostedDate(DateTime date)
    {
        LastPostedDate = date;
    }

    // Method to create a JournalEntry from the RecurringJournalEntry
    public JournalEntry CreateJournalEntry()
    {
        return new JournalEntry(Description, ReferenceNumber);
    }
}