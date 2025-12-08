using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public class VendorPaymentDto
{
    public Guid PaymentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionReference { get; set; }
    public string InvoiceNumber { get; set; } 
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; }
    //public PaymentMethod PaymentMethod { get; set; } // e.g., "ACH", "Cheque"
    public string Status { get; set; } // e.g., "Cleared", "Pending", "Failed"
}