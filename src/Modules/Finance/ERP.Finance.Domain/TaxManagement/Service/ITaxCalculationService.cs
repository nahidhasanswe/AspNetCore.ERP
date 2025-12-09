using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.TaxManagement.Service;

public interface ITaxCalculationService
{
    Task<Result<(Money TaxAmount, Guid TaxPayableAccountId)>> CalculateTax(
        Guid jurisdictionId,
        Money baseAmount,
        DateTime transactionDate,
        CancellationToken cancellationToken = default);
}