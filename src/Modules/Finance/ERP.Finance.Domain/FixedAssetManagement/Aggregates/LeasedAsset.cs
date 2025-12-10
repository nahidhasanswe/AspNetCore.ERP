using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public enum LeaseType
{
    OperatingLease,
    FinanceLease // Capital Lease
}

public enum LeaseStatus
{
    Active,
    Expired,
    Terminated
}

public class LeasedAsset : AggregateRoot
{
    public Guid BusinessUnitId { get; private set; }
    public Guid AssetId { get; private set; } // Link to a FixedAsset if tracked there
    public string LeaseAgreementNumber { get; private set; }
    public string Lessor { get; private set; }
    public LeaseType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Money MonthlyPayment { get; private set; }
    public LeaseStatus Status { get; private set; }

    private LeasedAsset() { }

    public LeasedAsset(Guid businessUnitId, Guid assetId, string leaseAgreementNumber, string lessor, LeaseType type, DateTime startDate, DateTime endDate, Money monthlyPayment) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        AssetId = assetId;
        LeaseAgreementNumber = leaseAgreementNumber;
        Lessor = lessor;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        MonthlyPayment = monthlyPayment;
        Status = LeaseStatus.Active;
    }

    public void Terminate(DateTime terminationDate, string reason)
    {
        if (Status == LeaseStatus.Terminated) return;
        Status = LeaseStatus.Terminated;
        // Optionally raise an event for LeaseTerminatedEvent
    }

    public void MarkAsExpired()
    {
        if (Status == LeaseStatus.Terminated) return;
        Status = LeaseStatus.Expired;
    }
}