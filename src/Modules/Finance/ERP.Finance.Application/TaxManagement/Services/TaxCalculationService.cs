using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Domain.TaxManagement.DTOs;
using ERP.Finance.Domain.TaxManagement.Service;

namespace ERP.Finance.Application.TaxManagement.Services;

public class TaxCalculationService(
    ITaxRateRepository repository
    ) : ITaxCalculationService
{
    public async Task<TaxCalculationResult> CalculateTax(TaxCalculationRequest request)
    {
        // 1. Find the applicable tax rate
        var taxRate = await repository.GetRateByJurisdictionAndDate(
            request.JurisdictionCode, 
            request.TransactionDate
        );

        if (taxRate is null)
        {
            // No tax applies for this jurisdiction/date.
            return new TaxCalculationResult 
            { 
                TaxAmount = new Money(0, request.BaseAmount.Currency),
                TaxPayableAccountId = Guid.Empty // Signifies no tax liability
            };
        }

        // 2. Perform the calculation (Domain Rule)
        decimal taxAmountValue = request.BaseAmount.Amount * taxRate.Rate;
        
        return new TaxCalculationResult
        {
            TaxAmount = new Money(taxAmountValue, request.BaseAmount.Currency),
            TaxPayableAccountId = taxRate.TaxPayableAccountId,
            SourceTransactionId = request.SourceTransactionId,
            IsSalesTransaction = request.IsSalesTransaction
        };
    }
}