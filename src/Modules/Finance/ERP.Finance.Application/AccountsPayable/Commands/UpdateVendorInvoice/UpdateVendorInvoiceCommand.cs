using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendorInvoice;

public class UpdateVendorInvoiceCommand : IRequestCommand<Unit>
{
    public Guid InvoiceId { get; set; }
    public DateTime NewDueDate { get; set; }
    public List<InvoiceLineItemDto> NewLineItems { get; set; } = new();

    public class InvoiceLineItemDto
    {
        public string Description { get; set; }
        public Money LineAmount { get; set; }
        public Guid ExpenseAccountId { get; set; }
        public Guid? CostCenterId { get; set; }
    }
}