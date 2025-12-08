using ERP.Core.Aggregates;

namespace ERP.Finance.Domain.Shared.Currency;

public class ExchangeRate : AggregateRoot
{
    public string FromCurrency { get; private set; } // e.g., "EUR"
    public string ToCurrency { get; private set; }   // e.g., "USD"
    public DateTime EffectiveDate { get; private set; }
    public decimal Rate { get; private set; } // e.g., 1.10 (1 EUR = 1.10 USD)

    private ExchangeRate() { }
    
    public ExchangeRate(string from, string to, DateTime date, decimal rate) : base(Guid.NewGuid())
    {
        if (rate <= 0) throw new ArgumentException("Rate must be positive.");
        
        FromCurrency = from.ToUpperInvariant();
        ToCurrency = to.ToUpperInvariant();
        EffectiveDate = date.Date;
        Rate = rate;
    }
}