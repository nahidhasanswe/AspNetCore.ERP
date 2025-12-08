using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.TaxManagement.DTOs;

public class TaxCalculationResult
{
    public Money TaxAmount { get; set; }
    public Guid TaxPayableAccountId { get; set; } // From the TaxRate entity
    
    // Additional data used for GL posting (needed in the Application layer)
    public Guid SourceTransactionId { get; set; }
    public bool IsSalesTransaction { get; set; }
}