namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public record VendorSpendAnalysisDto(
    Guid VendorId,
    string VendorName,
    Guid ExpenseAccountId,
    string ExpenseAccountName,
    decimal TotalSpendAmount,
    string Currency
);