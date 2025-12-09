using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CancelCustomerInvoice;

public class CancelCustomerInvoiceCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; }
}