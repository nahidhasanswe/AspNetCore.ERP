using System;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record VendorDto(
    Guid Id,
    string Name,
    string TaxId,
    string PaymentTerms,
    string Email,
    string Phone
);