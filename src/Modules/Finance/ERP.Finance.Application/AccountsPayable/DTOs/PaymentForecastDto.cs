namespace ERP.Finance.Application.AccountsPayable.DTOs;

public class PaymentForecastDto
{
    public Guid InvoiceId { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; }
    public DateTime DueDate { get; set; }
    public decimal OutstandingAmount { get; set; } // In transaction currency
    public string Currency { get; set; }
    public decimal AmountInBaseCurrency { get; set; } // For accurate reporting
}