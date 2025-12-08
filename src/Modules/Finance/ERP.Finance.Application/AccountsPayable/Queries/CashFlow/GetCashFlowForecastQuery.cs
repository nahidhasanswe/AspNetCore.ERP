using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.CashFlow;

public class GetCashFlowForecastQuery : IRequestCommand<List<PaymentForecastDto>>
{
    public int DaysAhead { get; set; } = 90;
}