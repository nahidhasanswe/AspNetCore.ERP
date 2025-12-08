using ERP.Core.Repository;

namespace ERP.Finance.Domain.Shared.Currency;

public interface IExchangeRateRepository : IRepository<ExchangeRate>
{
    Task<ExchangeRate> GetRateAsync(string fromCurrency, string toCurrency, DateTime date); 
}