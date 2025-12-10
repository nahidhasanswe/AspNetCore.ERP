using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendorInvoice;

public class UpdateVendorInvoiceCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
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