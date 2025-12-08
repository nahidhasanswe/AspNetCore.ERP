using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record VendorOnboardingRequestDetailsDto(
    Guid Id,
    string ProposedName,
    string ProposedTaxId,
    AddressDto ProposedAddress,
    ContactInfoDto ProposedContactInfo,
    string ProposedPaymentTerms,
    string ProposedDefaultCurrency,
    VendorBankDetailsDto ProposedBankDetails,
    OnboardingStatus Status,
    string RejectionReason,
    Guid? ApprovedVendorId,
    DateTime CreatedDate
);