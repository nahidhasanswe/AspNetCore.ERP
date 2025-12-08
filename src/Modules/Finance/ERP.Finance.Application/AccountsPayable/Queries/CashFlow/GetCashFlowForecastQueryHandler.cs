using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Service;
using ERP.Finance.Domain.Shared.Currency;

namespace ERP.Finance.Application.AccountsPayable.Queries.CashFlow;

public class GetCashFlowForecastQueryHandler(
    IVendorInvoiceRepository repository,
    ICurrencyConversionService currencyConverter,
    IVendorLookupService vendorLookupService
    ) : IRequestCommandHandler<GetCashFlowForecastQuery, List<PaymentForecastDto>>
{
    private const string SystemBaseCurrency = "USD"; 

    public async Task<Result<List<PaymentForecastDto>>> Handle(GetCashFlowForecastQuery query, CancellationToken cancellationToken)
    {
        var dueDateCutoff = DateTime.Today.AddDays(query.DaysAhead);

        // 1. Fetch Projections (Requires a repository method to get open invoices)
        // NOTE: In a real system, this would use a highly optimized Read Model (Projection) DB.
        // We'll assume the repository has a projection method:
        var projections = await repository.GetForecastProjectionAsync(dueDateCutoff); // Assumed method

        var results = new List<PaymentForecastDto>();
        
        // 2. Process and Convert for Accurate Forecasting
        foreach (var proj in projections)
        {
            // Convert outstanding amount to Base Currency for consolidated forecasting
            var baseMoney = await currencyConverter.ConvertAsync(
                source: proj.OutstandingBalance,
                targetCurrency: SystemBaseCurrency,
                conversionDate: DateTime.Today // Use today's rate for forecast
            );

            var vendorName = await vendorLookupService.GetVendorNameAsync(proj.VendorId);

            results.Add(new PaymentForecastDto
            {
                InvoiceId = proj.Id,
                VendorId = proj.VendorId,
                VendorName = vendorName,
                DueDate = proj.DueDate,
                OutstandingAmount = proj.OutstandingBalance.Amount,
                Currency = proj.OutstandingBalance.Currency,
                AmountInBaseCurrency = baseMoney.Amount
            });
        }

        return Result.Success(results.OrderBy(r => r.DueDate).ToList());
    }
}