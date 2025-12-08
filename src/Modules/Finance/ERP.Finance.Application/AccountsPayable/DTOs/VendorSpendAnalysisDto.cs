using System;

namespace ERP.Finance.Application.AccountsPayable.DTOs;

public record VendorSpendAnalysisDto(
    Guid VendorId,
    string VendorName,
    Guid ExpenseAccountId,
    string ExpenseAccountName, // Assuming we can get this from somewhere
    decimal TotalSpendAmount,
    string Currency
);