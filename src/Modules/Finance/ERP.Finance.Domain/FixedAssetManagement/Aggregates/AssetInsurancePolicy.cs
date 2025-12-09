using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public enum PolicyStatus
{
    Active,
    Expired,
    Cancelled
}

public class AssetInsurancePolicy : AggregateRoot
{
    public Guid AssetId { get; private set; }
    public string PolicyNumber { get; private set; }
    public string Insurer { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Money CoverageAmount { get; private set; }
    public Money PremiumAmount { get; private set; }
    public PolicyStatus Status { get; private set; }

    private AssetInsurancePolicy() { }

    public AssetInsurancePolicy(Guid assetId, string policyNumber, string insurer, DateTime startDate, DateTime endDate, Money coverageAmount, Money premiumAmount) : base(Guid.NewGuid())
    {
        AssetId = assetId;
        PolicyNumber = policyNumber;
        Insurer = insurer;
        StartDate = startDate;
        EndDate = endDate;
        CoverageAmount = coverageAmount;
        PremiumAmount = premiumAmount;
        Status = PolicyStatus.Active;
    }

    public void Renew(DateTime newEndDate, Money newCoverageAmount, Money newPremiumAmount)
    {
        if (Status == PolicyStatus.Cancelled)
            throw new DomainException("Cannot renew a cancelled policy.");
        
        EndDate = newEndDate;
        CoverageAmount = newCoverageAmount;
        PremiumAmount = newPremiumAmount;
        Status = PolicyStatus.Active;
        // Optionally raise an event for PolicyRenewedEvent
    }

    public void Cancel(string reason)
    {
        if (Status == PolicyStatus.Cancelled) return;
        Status = PolicyStatus.Cancelled;
        // Optionally raise an event for PolicyCancelledEvent
    }

    public void MarkAsExpired()
    {
        if (Status == PolicyStatus.Cancelled) return;
        Status = PolicyStatus.Expired;
    }
}