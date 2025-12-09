using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Domain.TaxManagement.Service;

namespace ERP.Finance.Application.TaxManagement.Services;

public class TaxCalculationService(ITaxRateRepository taxRateRepository) : ITaxCalculationService
{
    public async Task<Result<(Money TaxAmount, Guid TaxPayableAccountId)>> CalculateTax(
        Guid jurisdictionId,
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

        var taxAmount = new Money(baseAmount.Amount * applicableRate.Rate, baseAmount.Currency);
        return Result.Success((taxAmount, applicableRate.TaxPayableAccountId));
    }
}