using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public record VendorInvoiceSummaryDto(
    Guid Id,
    string InvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    decimal TotalAmount,
    decimal OutstandingBalance,
    InvoiceStatus Status
);