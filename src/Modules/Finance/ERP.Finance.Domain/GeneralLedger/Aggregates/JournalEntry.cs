using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Events;
using ERP.Finance.Domain.GeneralLedger.Services;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class JournalEntry : AggregateRoot
{
    public Guid BusinessUnitId { get; set; }
    public DateTime PostingDate { get; private set; }
    public string Description { get; private set; }
    public string ReferenceNumber { get; private set; }
    public bool IsPosted { get; private set; }

    private readonly List<LedgerLine> _lines = new();
    public IReadOnlyCollection<LedgerLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { } 

    public JournalEntry(string description, string referenceNumber, Guid businessUnitId) : base(Guid.NewGuid())
    {
        Description = description;
        ReferenceNumber = referenceNumber;
        IsPosted = false;
        PostingDate = default; 
        
    }

    public void AddLine(LedgerLine line)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot modify a posted entry.");
        _lines.Add(line);
    }
    
    public void Post(FiscalPeriod period, IAccountValidationService accountValidator, bool isClosingEntry = false)
    {
        if (IsPosted) return;

        // 0. Check Fiscal Period status
        if (isClosingEntry)
        {
            if (period.Status != FiscalYear.Enums.PeriodStatus.SoftClose)
                throw new DomainException($"A closing entry can only be posted to a soft-closed period. Period '{period.Name}' has status '{period.Status}'.");
        }
        else
        {
            if (period.Status != FiscalYear.Enums.PeriodStatus.Open)
                throw new DomainException($"Cannot post to a period that is not open. Period '{period.Name}' has status '{period.Status}'.");
        }

        var totalDebits = _lines.Where(l => l.IsDebit).Sum(l => l.Amount.Amount);
        var totalCredits = _lines.Where(l => !l.IsDebit).Sum(l => l.Amount.Amount);
        
        // 1. Check for Balance Invariant
        if (!_lines.Any())
        {
            throw new DomainException("Cannot post a journal entry with no lines.");
        }
        if (totalDebits != totalCredits)
        {
            throw new DomainException($"Journal Entry {Id} is unbalanced. Total debits ({totalDebits}) do not equal total credits ({totalCredits}).");
        }

        // 2. Check for Currency Consistency Invariant
        var currency = _lines.First().Amount.Currency;
        if (_lines.Any(l => l.Amount.Currency != currency))
        {
            throw new DomainException($"Journal Entry {Id} contains mixed currencies. All lines must be in '{currency}'.");
        }

        // 3. Check Account validity for each line
        foreach (var line in _lines)
        {
            accountValidator.ValidatePostingAccount(line.AccountId);
        }

        IsPosted = true;
        PostingDate = DateTime.UtcNow;

        // Ensure posting date is within the fiscal period
        if (PostingDate < period.StartDate || PostingDate > period.EndDate)
            PostingDate = period.EndDate; // Or throw an exception, depending on business rules
        
        AddDomainEvent(new JournalEntryPostedEvent(this.Id.ToString())); 
    }
}