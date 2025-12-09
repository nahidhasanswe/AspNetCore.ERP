using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CashReceiptDetailsDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName, // Will be populated by handler
    DateTime ReceiptDate,
    Money TotalReceivedAmount,
    Money TotalAppliedAmount,
    Money UnappliedAmount,
    string TransactionReference,
    Guid CashAccountId,
    string CashAccountName, // Will be populated by handler
    ReceiptStatus Status
);