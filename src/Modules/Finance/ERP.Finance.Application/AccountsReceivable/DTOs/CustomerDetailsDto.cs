using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerDetailsDto(
    Guid Id,
    string Name,
    string ContactEmail,
    AddressDto BillingAddress,
    ContactInfoDto ContactInfo,
    string PaymentTerms,
    string DefaultCurrency,
    Guid ARControlAccountId,
    CustomerStatus Status,
    Guid? CustomerCreditProfileId,
    CustomerCreditProfileDto? CreditProfile // Nested DTO for credit profile
);

public record CustomerCreditProfileDto(
    Guid Id,
    Money ApprovedLimit,
    decimal CurrentExposure,
    CreditStatus Status,
    string DefaultPaymentTerms
);