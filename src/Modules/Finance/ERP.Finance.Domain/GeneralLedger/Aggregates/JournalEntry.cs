using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.GeneralLedger.Events;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class JournalEntry : AggregateRoot
{
    public DateTime PostingDate { get; private set; }
    public string Description { get; private set; }
    public string ReferenceNumber { get; private set; }
    public bool IsPosted { get; private set; }

    private readonly List<LedgerLine> _lines = new();
    public IReadOnlyCollection<LedgerLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { } 

    public JournalEntry(string description, string referenceNumber) : base(Guid.NewGuid())
    {
        Description = description;
        ReferenceNumber = referenceNumber;
        IsPosted = false;
        PostingDate = default; // Will be set on Post()
    }

    public void AddLine(LedgerLine line)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot modify a posted entry.");
        _lines.Add(line);
    }
    
    public void Post()
    {
        if (IsPosted) return;

        var totalDebits = _lines.Where(l => l.IsDebit).Sum(l => l.Amount.Amount);
        var totalCredits = _lines.Where(l => !l.IsDebit).Sum(l => l.Amount.Amount);
        
        // 1. Check for Balance Invariant
        if (totalDebits != totalCredits)
        {
            throw new DomainException($"Entry is unbalanced: Debit {totalDebits} != Credit {totalCredits}.");
        }

        // 2. Check for Currency Consistency Invariant
        var currency = _lines.First().Amount.Currency;
        if (_lines.Any(l => l.Amount.Currency != currency))
        {
            throw new DomainException("All ledger lines must use the same currency.");
        }

        IsPosted = true;
        PostingDate = DateTime.UtcNow;
        AddDomainEvent(new JournalEntryPostedEvent(this.Id.ToString())); 
    }
}