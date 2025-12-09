using ERP.Core;
using ERP.Finance.Domain.Budgeting.Aggregates; // Added for Budget aggregate return type
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.Budgeting.Service;

public interface IBudgetService
{
    Task<Result> CheckFundsAvailability(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount);
    Task<Result<Budget>> ReserveFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId);
    Task<Result<Budget>> ReleaseFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId);
    Task<Result<Budget>> LiquidateFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId);
}