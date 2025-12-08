using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class PurchaseOrderLine : Entity
{
    public Guid ProductId { get; private set; }
    public string Description { get; private set; }
    public decimal Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice => new(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    private PurchaseOrderLine() { }

    public PurchaseOrderLine(Guid productId, string description, decimal quantity, Money unitPrice) : base(Guid.NewGuid())
    {
        ProductId = productId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}