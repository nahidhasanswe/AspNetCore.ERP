using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerInvoiceDetailsDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName, // Will be populated by handler
    DateTime IssueDate,
    DateTime DueDate,
    Money TotalAmount,
    Money OutstandingBalance,
    InvoiceStatus Status,
    string InvoiceNumber,
    Guid ARControlAccountId,
    Guid? CostCenterId,
    decimal TotalPaymentsReceived,
    decimal TotalAmountWrittenOff,
    decimal TotalCreditsApplied,
    IReadOnlyCollection<CustomerInvoiceLineItemDto> LineItems
);

public record CustomerInvoiceLineItemDto(
    Guid Id,
    string Description,
    Money LineAmount,
    Guid RevenueAccountId,
    string RevenueAccountName, // Will be populated by handler
    Guid? CostCenterId
);