using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommand : IRequest<Result<Guid>>
{
    public Guid VendorId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<PurchaseOrderLineDto> Lines { get; set; } = new();

    public class PurchaseOrderLineDto
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public Money UnitPrice { get; set; }
    }
}