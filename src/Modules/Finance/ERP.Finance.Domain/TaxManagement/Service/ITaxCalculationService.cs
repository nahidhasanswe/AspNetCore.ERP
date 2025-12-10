using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.TaxManagement.Service;

public interface ITaxCalculationService
{
    Task<Result<(Money TaxAmount, Guid TaxPayableAccountId)>> CalculateTax(
        Guid jurisdictionId,
        Guid businessUnitId,
        Money baseAmount,
        DateTime transactionDate,
        CancellationToken cancellationToken = default);
}