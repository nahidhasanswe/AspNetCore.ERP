using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public enum MaintenanceStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public class AssetMaintenanceRecord : AggregateRoot
{
    public Guid AssetId { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public string Description { get; private set; }
    public Money Cost { get; private set; }
    public string PerformedBy { get; private set; }
    public MaintenanceStatus Status { get; private set; }

    private AssetMaintenanceRecord() { }

    public AssetMaintenanceRecord(Guid assetId, DateTime scheduledDate, string description, Money cost, string performedBy) : base(Guid.NewGuid())
    {
        AssetId = assetId;
        ScheduledDate = scheduledDate;
        Description = description;
        Cost = cost;
        PerformedBy = performedBy;
        Status = MaintenanceStatus.Scheduled;
    }

    public void MarkInProgress()
    {
        if (Status != MaintenanceStatus.Scheduled)
            throw new DomainException("Only scheduled maintenance can be marked in progress.");
        Status = MaintenanceStatus.InProgress;
    }

    public void MarkCompleted(DateTime completionDate)
    {
        if (Status != MaintenanceStatus.InProgress)
            throw new DomainException("Only in-progress maintenance can be marked completed.");
        CompletionDate = completionDate;
        Status = MaintenanceStatus.Completed;
        // Optionally raise an event for MaintenanceCompletedEvent
    }

    public void Cancel(string reason)
    {
        if (Status == MaintenanceStatus.Completed)
            throw new DomainException("Cannot cancel completed maintenance.");
        Status = MaintenanceStatus.Cancelled;
        // Optionally raise an event for MaintenanceCancelledEvent
    }
}