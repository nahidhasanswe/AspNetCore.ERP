using ERP.Core;
using ERP.Finance.Domain.GeneralLedger.Services; // For IGLConfigurationService
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Domain.TaxManagement.Service;

namespace ERP.Finance.Application.TaxManagement.Services;

public class TaxCalculationService(ITaxRateRepository taxRateRepository, IGLConfigurationService glConfigurationService)
    : ITaxCalculationService
{
    public async Task<Result<(Money TaxAmount, Guid TaxPayableAccountId)>> CalculateTax(
        Guid jurisdictionId,
        Guid businessUnitId, // Added this parameter
        Money baseAmount,
        DateTime transactionDate,
        CancellationToken cancellationToken = default)
    {
        var applicableRate = (await taxRateRepository.ListAllAsync(cancellationToken))
            .FirstOrDefault(tr => tr.JurisdictionId == jurisdictionId &&
                                 tr.EffectiveDate <= transactionDate &&
                                 tr.IsActive); // Ensure rate is active

        if (applicableRate == null)
        {
            return Result.Failure<(Money TaxAmount, Guid TaxPayableAccountId)>("No applicable tax rate found for the given jurisdiction and date.");
        }

        // Dynamically resolve TaxPayableAccountId based on jurisdiction and business unit
        var taxPayableAccountId = await glConfigurationService.GetTaxPayableAccountId(jurisdictionId, businessUnitId);
        if (taxPayableAccountId == Guid.Empty) // Assuming Guid.Empty means not found
        {
            return Result.Failure<(Money TaxAmount, Guid TaxPayableAccountId)>($"Tax Payable GL Account not configured for jurisdiction {jurisdictionId} and business unit {businessUnitId}.");
        }

        var taxAmount = new Money(baseAmount.Amount * applicableRate.Rate, baseAmount.Currency);
        return Result.Success((taxAmount, taxPayableAccountId));
    }
}