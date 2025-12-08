using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class LedgerLine : Entity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public Money BaseAmount { get; private set; }
    public bool IsDebit { get; private set; } // True: Debit, False: Credit
    public string Description { get; private set; }
    
    public Guid? CostCenterId { get; private set; }

    // Private constructor for EF Core
    private LedgerLine() { } 
    
    public LedgerLine(
        Guid journalEntryId, // Added for FK
        Guid accountId, 
        Money amount, 
        Money baseAmount, 
        bool isDebit, 
        string description,
        Guid? costCenterId = null // Optional dimension
    ) : base(Guid.NewGuid())
    {
        if (amount.Amount <= 0)
             throw new ArgumentException("Ledger line amount must be positive.");
        
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Amount = amount;
        BaseAmount = baseAmount;
        IsDebit = isDebit;
        Description = description;
        CostCenterId = costCenterId;
    }
}