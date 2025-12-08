using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record VendorOnboardingRequestSummaryDto(
    Guid Id,
    string ProposedName,
    string ProposedTaxId,
    OnboardingStatus Status,
    DateTime CreatedDate // Assuming AggregateRoot has a CreatedDate
);