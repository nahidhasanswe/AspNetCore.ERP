using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendor;

public class UpdateVendorInvoiceCommand : IRequestCommand<bool>
{
    public Guid InvoiceId { get; set; }
    public DateTime NewDueDate { get; set; }
    public IEnumerable<UpdatedInvoiceLineItemDto> UpdatedLines { get; set; } 
}

public class UpdatedInvoiceLineItemDto
{
    public string Description { get; set; }
    public Money LineAmount { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public Guid? CostCenterId { get; set; }
}