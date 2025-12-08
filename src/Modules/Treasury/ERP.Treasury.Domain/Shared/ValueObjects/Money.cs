using ERP.Core.ValueObjects;

namespace ERP.Treasury.Domain.Shared.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } // e.g., "USD", "EUR"

    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));
        
        // Negative amounts are handled by LedgerLine (IsDebit flag), not here.
        Amount = Math.Abs(amount); 
        Currency = currency.Trim().ToUpper();
    }
    
    // Private constructor for EF Core or deserialization
    private Money() {} 

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}