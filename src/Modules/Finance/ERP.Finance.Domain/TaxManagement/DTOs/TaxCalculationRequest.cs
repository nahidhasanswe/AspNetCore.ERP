using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.TaxManagement.DTOs;

public class TaxCalculationRequest
{
    public Money BaseAmount { get; set; }
    public string JurisdictionCode { get; set; } // To determine the rate
    public DateTime TransactionDate { get; set; }
    public Guid SourceTransactionId { get; set; } // AR Invoice or AP Bill ID
    public bool IsSalesTransaction { get; set; } // Determines Debit/Credit logic
}