using ERP.Core;
using MediatR;
using System;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.ReceiveGoodsOnPO;

public class ReceiveGoodsOnPOCommand : IRequestCommand<Unit>
{
    public Guid PurchaseOrderId { get; set; }
    public Guid PurchaseOrderLineId { get; set; }
    public decimal QuantityReceived { get; set; }
}