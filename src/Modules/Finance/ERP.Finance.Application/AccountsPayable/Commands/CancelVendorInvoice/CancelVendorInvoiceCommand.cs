using ERP.Core;
using MediatR;
using System;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelVendorInvoice;

public class CancelVendorInvoiceCommand : IRequestCommand<bool>
{
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; }
}