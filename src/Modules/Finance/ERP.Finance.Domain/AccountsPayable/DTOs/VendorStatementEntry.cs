namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public class VendorStatementEntry
{
    public DateTime TransactionDate { get; set; }
    public string ReferenceNumber { get; set; } // Invoice #, Payment Ref, Credit Note #
    public string Type { get; set; } // "Invoice", "Payment", "CreditNote"
    public decimal DebitAmount { get; set; } // Usually Payments/Credit Notes
    public decimal CreditAmount { get; set; } // Usually Invoices
    public decimal RunningBalance { get; set; } // Essential for reconciliation
    public string Currency { get; set; }
}