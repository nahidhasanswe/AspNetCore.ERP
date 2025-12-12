using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum OnboardingStatus
{
    Submitted,
    PendingApproval,
    Approved,
    Rejected,
    Completed // Vendor created
}

public class VendorOnboardingRequest : AuditableAggregateRoot
{
    public string ProposedName { get; private set; }
    public string ProposedTaxId { get; private set; }
    public Address ProposedAddress { get; private set; }
    public ContactInfo ProposedContactInfo { get; private set; }
    public string ProposedPaymentTerms { get; private set; }
    public string ProposedDefaultCurrency { get; private set; }
    public VendorBankDetails ProposedBankDetails { get; private set; }
    public OnboardingStatus Status { get; private set; }
    public string RejectionReason { get; private set; }
    public Guid? ApprovedVendorId { get; private set; }

    private VendorOnboardingRequest() { }

    public VendorOnboardingRequest(
        string proposedName,
        string proposedTaxId,
        Address proposedAddress,
        ContactInfo proposedContactInfo,
        string proposedPaymentTerms,
        string proposedDefaultCurrency,
        VendorBankDetails proposedBankDetails) : base(Guid.NewGuid())
    {
        ProposedName = proposedName;
        ProposedTaxId = proposedTaxId;
        ProposedAddress = proposedAddress;
        ProposedContactInfo = proposedContactInfo;
        ProposedPaymentTerms = proposedPaymentTerms;
        ProposedDefaultCurrency = proposedDefaultCurrency;
        ProposedBankDetails = proposedBankDetails;
        Status = OnboardingStatus.Submitted;
        CreatedAt = DateTime.UtcNow;
    }

    public void SubmitForApproval()
    {
        if (Status != OnboardingStatus.Submitted)
            throw new DomainException("Only submitted requests can be sent for approval.");
        Status = OnboardingStatus.PendingApproval;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedVendorId)
    {
        if (Status != OnboardingStatus.PendingApproval)
            throw new DomainException("Only pending approval requests can be approved.");
        ApprovedVendorId = approvedVendorId;
        Status = OnboardingStatus.Approved;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != OnboardingStatus.PendingApproval)
            throw new DomainException("Only pending approval requests can be rejected.");
        RejectionReason = reason;
        Status = OnboardingStatus.Rejected;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        if (Status != OnboardingStatus.Approved)
            throw new DomainException("Only approved requests can be marked as completed.");
        Status = OnboardingStatus.Completed;
        LastModifiedAt = DateTime.UtcNow;
    }
}