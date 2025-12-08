using System;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record CashRequirementDto(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid VendorId,
    string VendorName,
    DateTime DueDate,
    decimal AmountDue,
    string Currency,
    string Status // Current status of the invoice
);