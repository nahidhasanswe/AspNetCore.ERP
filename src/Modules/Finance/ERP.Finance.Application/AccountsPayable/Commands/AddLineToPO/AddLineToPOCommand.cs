using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.AddLineToPO;

public class AddLineToPOCommand : IRequestCommand<Unit>
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public Money UnitPrice { get; set; }
}