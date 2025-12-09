using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CashReceiptSummaryDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    DateTime ReceiptDate,
    Money TotalReceivedAmount,
    Money UnappliedAmount,
    ReceiptStatus Status
);