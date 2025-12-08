using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.Shared.Currency;

public class CurrencyConversionService(IExchangeRateRepository repository) : ICurrencyConversionService
{
    // CRITICAL: This must be the system's Base Reporting Currency, loaded from config.
    private const string SystemBaseCurrency = "USD";

    /// <summary>
    /// Converts a Money object from its current currency to a target currency 
    /// using the triangulation method (via the SystemBaseCurrency).
    /// </summary>
    public async Task<Money> ConvertAsync(Money source, string targetCurrency, DateTime conversionDate)
    {
        string sourceCurrency = source.Currency.ToUpperInvariant();
        targetCurrency = targetCurrency.ToUpperInvariant();

        if (sourceCurrency == targetCurrency)
        {
            return source;
        }

        // Helper function to get the rate from any currency to the base currency (e.g., EUR to USD).
        Func<string, Task<decimal>> getRateToBase = async (currency) =>
        {
            if (currency == SystemBaseCurrency) return 1.0m;
            
            var rateEntity = await repository.GetRateAsync(currency, SystemBaseCurrency, conversionDate);
            if (rateEntity != null)
                return rateEntity.Rate; // Direct rate (e.g., EUR -> USD)

            // If we store rates inversely (e.g., USD -> EUR), calculate the inverse.
            var inverseRateEntity = await repository.GetRateAsync(SystemBaseCurrency, currency, conversionDate);
            if (inverseRateEntity != null && inverseRateEntity.Rate != 0)
                return 1.0m / inverseRateEntity.Rate; // Inverse rate (1 / USD->EUR)

            return 0m; // Rate not found
        };

        // 1. Get the rate to convert the source currency to the base currency
        decimal rateSourceToBase = await getRateToBase(sourceCurrency);
        
        // 2. Get the rate to convert the target currency to the base currency
        decimal rateTargetToBase = await getRateToBase(targetCurrency);

        if (rateSourceToBase == 0m || rateTargetToBase == 0m)
            throw new DomainException($"Exchange rate not available for conversion involving {sourceCurrency} or {targetCurrency} on {conversionDate.ToShortDateString()}.");

        // --- Triangulation Calculation ---
        
        // Step A: Convert Source Amount to Base Currency
        // (e.g., 100 EUR * 1.10 EUR/USD = 110 USD)
        decimal amountInBase = source.Amount * rateSourceToBase;

        // Step B: Convert Base Currency Amount to Target Currency
        // (e.g., 110 USD / 0.90 USD/GBP = 122.22 GBP)
        decimal finalAmount = amountInBase / rateTargetToBase;

        // Finalize (Apply banking/accounting rounding rules)
        finalAmount = Math.Round(finalAmount, 4); 

        return new Money(finalAmount, targetCurrency);
    }
}