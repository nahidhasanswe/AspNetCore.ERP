using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.Currency;

namespace ERP.Finance.Application.AccountsPayable.Queries.CashFlow;

public class GetCashFlowForecastQueryHandler(
    IVendorInvoiceRepository repository,
    ICurrencyConversionService currencyConverter
    ) : IRequestCommandHandler<GetCashFlowForecastQuery, List<PaymentForecastDto>>
{
    private const string SystemBaseCurrency = "USD"; 

    public async Task<Result<List<PaymentForecastDto>>> Handle(GetCashFlowForecastQuery query, CancellationToken cancellationToken)
    {
        var dueDateCutoff = DateTime.Today.AddDays(query.DaysAhead);

        var projections = await repository.GetForecastProjectionAsync(dueDateCutoff, query.BusinessUnitId, cancellationToken);

        var tasks = projections.Select(async proj =>
        {
            // Perform the async operation
            var baseMoney = await currencyConverter.ConvertAsync(
                source: proj.OutstandingBalance,
                targetCurrency: SystemBaseCurrency,
                conversionDate: DateTime.Today
            );

            // Return the constructed DTO
            return new PaymentForecastDto
            {
                InvoiceId = proj.Id,
                VendorId = proj.VendorId,
                VendorName = proj.VendorName,
                DueDate = proj.DueDate,
                OutstandingAmount = proj.OutstandingBalance.Amount,
                Currency = proj.OutstandingBalance.Currency,
                AmountInBaseCurrency = baseMoney.Amount
            };
        });
        
        // Await all tasks to complete concurrently
        var resultsArray = await Task.WhenAll(tasks);
        
        return Result.Success(resultsArray.ToList());
    }
}