using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public class CashFlowProjectionDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; }
    public DateTime DueDate { get; set; }
    public Money OutstandingBalance { get; set; }
    public Guid BusinessUnitId { get; set; }
    public InvoiceStatus Status { get; set; }
}