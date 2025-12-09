using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerStatementDto(
    Guid CustomerId,
    string CustomerName,
    DateTime StatementDate,
    DateTime StartDate,
    DateTime EndDate,
    decimal BeginningBalance,
    decimal EndingBalance,
    string Currency,
    IReadOnlyCollection<CustomerStatementTransactionDto> Transactions
);

public record CustomerStatementTransactionDto(
    DateTime TransactionDate,
    string Type, // e.g., "Invoice", "Payment", "Credit Memo", "Adjustment"
    string Reference, // Invoice Number, Receipt Number, Memo Number
    string Description,
    decimal Debit, // Increase AR (e.g., Invoice)
    decimal Credit, // Decrease AR (e.g., Payment, Credit Memo)
    decimal RunningBalance
);