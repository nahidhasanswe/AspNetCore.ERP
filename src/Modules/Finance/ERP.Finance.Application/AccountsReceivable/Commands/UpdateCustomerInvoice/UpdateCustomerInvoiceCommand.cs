using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomerInvoice;

public class UpdateCustomerInvoiceCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public DateTime NewDueDate { get; set; }
    public Guid? NewCostCenterId { get; set; }
    // Add other updatable header fields as needed
}