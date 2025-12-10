using ERP.Core.Aggregates;
using ERP.Core.Exceptions;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public enum InventoryStatus
{
    Counted,
    Reconciled,
    Adjusted
}

public class PhysicalInventoryRecord : AggregateRoot
{
    public Guid BusinessUnitId { get; private set; } // New property
    public Guid AssetId { get; private set; }
    public DateTime CountDate { get; private set; }
    public string CountedLocation { get; private set; }
    public string CountedBy { get; private set; } // User who performed the count
    public InventoryStatus Status { get; private set; }
    public string Notes { get; private set; }

    private PhysicalInventoryRecord() { }

    public PhysicalInventoryRecord(Guid businessUnitId, Guid assetId, DateTime countDate, string countedLocation, string countedBy, string notes) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        AssetId = assetId;
        CountDate = countDate;
        CountedLocation = countedLocation;
        CountedBy = countedBy;
        Notes = notes;
        Status = InventoryStatus.Counted;
    }

    public void MarkAsReconciled()
    {
        if (Status != InventoryStatus.Counted)
            throw new DomainException("Only counted records can be reconciled.");
        Status = InventoryStatus.Reconciled;
    }

    public void MarkAsAdjusted()
    {
        if (Status != InventoryStatus.Reconciled)
            throw new DomainException("Only reconciled records can be marked as adjusted.");
        Status = InventoryStatus.Adjusted;
    }
}