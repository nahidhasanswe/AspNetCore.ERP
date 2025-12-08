namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public class VendorStatementSummary
{
    public decimal BeginningBalance { get; set; }
    public decimal TotalInvoices { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal EndingBalance { get; set; }
    public string Currency { get; set; }
}