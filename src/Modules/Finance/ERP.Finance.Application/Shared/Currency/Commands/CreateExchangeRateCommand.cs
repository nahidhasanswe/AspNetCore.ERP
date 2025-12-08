using ERP.Core.Behaviors;

namespace ERP.Finance.Application.Shared.Currency.Commands;

public class CreateExchangeRateCommand : IRequestCommand<Guid>
{
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal Rate { get; set; } // The exchange rate
}