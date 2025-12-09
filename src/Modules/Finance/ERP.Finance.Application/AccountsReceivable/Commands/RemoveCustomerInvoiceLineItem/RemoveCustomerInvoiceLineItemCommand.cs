using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.RemoveCustomerInvoiceLineItem;

public class RemoveCustomerInvoiceLineItemCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public Guid LineItemId { get; set; }
}