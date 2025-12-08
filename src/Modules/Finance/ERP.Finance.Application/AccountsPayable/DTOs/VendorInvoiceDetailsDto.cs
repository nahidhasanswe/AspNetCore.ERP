using System;
using System.Collections.Generic;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record VendorInvoiceDetailsDto(
    Guid Id,
    Guid VendorId,
    string InvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    Money TotalAmount,
    Money OutstandingBalance,
    InvoiceStatus Status,
    bool IsOnHold,
    IReadOnlyCollection<InvoiceLineItemDto> LineItems
);

public record InvoiceLineItemDto(
    Guid Id,
    string Description,
    Money LineAmount,
    Guid ExpenseAccountId,
    Guid? CostCenterId
);