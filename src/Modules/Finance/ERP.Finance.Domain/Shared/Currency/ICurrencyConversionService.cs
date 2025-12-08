using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Shared.Currency;

public interface ICurrencyConversionService
{
    // Converts a source Money object to a target currency using the rate at a specific date.
    Task<Money> ConvertAsync(Money source, string targetCurrency, DateTime conversionDate);
}