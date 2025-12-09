using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerInvoiceSummaryDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName, // Will be populated by handler
    string InvoiceNumber,
    DateTime IssueDate,
    DateTime DueDate,
    Money TotalAmount,
    Money OutstandingBalance,
    InvoiceStatus Status
);