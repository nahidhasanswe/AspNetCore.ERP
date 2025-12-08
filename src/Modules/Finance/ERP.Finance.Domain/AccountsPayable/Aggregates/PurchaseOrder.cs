using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum POStatus
{
    Open,
    PartiallyReceived,
    FullyReceived,
    Closed
}

public class PurchaseOrder : AggregateRoot
{
    public Guid VendorId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public POStatus Status { get; private set; }

    private readonly List<PurchaseOrderLine> _lines = new();
    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines.AsReadOnly();

    // For 3-way matching, we track received quantities
    private readonly Dictionary<Guid, decimal> _receivedQuantities = new();

    private PurchaseOrder() { }

    public PurchaseOrder(Guid vendorId, DateTime orderDate, IEnumerable<PurchaseOrderLine> lines) : base(Guid.NewGuid())
    {
        VendorId = vendorId;
        OrderDate = orderDate;
        Status = POStatus.Open;
        _lines.AddRange(lines);
    }

    public void ReceiveGoods(Guid poLineId, decimal quantityReceived)
    {
        var line = _lines.FirstOrDefault(l => l.Id == poLineId);
        if (line == null) throw new DomainException("Purchase order line not found.");

        if (quantityReceived <= 0) throw new DomainException("Quantity received must be positive.");

        _receivedQuantities.TryGetValue(poLineId, out var alreadyReceived);
        if (alreadyReceived + quantityReceived > line.Quantity)
        {
            throw new DomainException("Cannot receive more than the ordered quantity.");
        }

        _receivedQuantities[poLineId] = alreadyReceived + quantityReceived;

        UpdateStatus();
    }

    public decimal GetReceivedQuantity(Guid poLineId)
    {
        _receivedQuantities.TryGetValue(poLineId, out var received);
        return received;
    }

    private void UpdateStatus()
    {
        var totalOrdered = _lines.Sum(l => l.Quantity);
        var totalReceived = _receivedQuantities.Values.Sum();

        if (totalReceived == 0)
            Status = POStatus.Open;
        else if (totalReceived < totalOrdered)
            Status = POStatus.PartiallyReceived;
        else
            Status = POStatus.FullyReceived;
    }
}