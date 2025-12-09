using ERP.Core.Events;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record FixedAssetAcquiredViaInvoiceEvent(
    Guid InvoiceId,
    Guid VendorId,
    string AssetTagNumber,
    string AssetDescription,
    DateTime AcquisitionDate,
    Money AcquisitionCost,
    Guid AssetAccountId, // GL Account for the Asset
    Guid DepreciationExpenseAccountId, // GL Account for Depreciation Expense
    Guid AccumulatedDepreciationAccountId, // GL Account for Accumulated Depreciation
    DepreciationMethod DepreciationMethod,
    int UsefulLifeYears,
    decimal SalvageValue,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}