using ERP.Core;
using MediatR;
using System;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.MatchInvoiceToPO;

public class MatchInvoiceToPOCommand : IRequestCommand<Unit>
{
    public Guid InvoiceId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public bool Perform3WayMatch { get; set; }
}