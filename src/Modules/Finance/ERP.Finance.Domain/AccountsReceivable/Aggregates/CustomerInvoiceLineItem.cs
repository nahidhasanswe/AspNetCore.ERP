using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CustomerInvoiceLineItem : Entity
{
    public string Description { get; private set; }
    public Money LineAmount { get; private set; }
    public Guid RevenueAccountId { get; private set; } // GL Account ID for the expense
    
    public Guid? CostCenterId { get; private set; }
    
    private CustomerInvoiceLineItem() { }
    
    public CustomerInvoiceLineItem(string description, Money lineAmount, Guid revenueAccountId, Guid? costCenterId) : base(Guid.NewGuid())
    {
        Description = description;
        LineAmount = lineAmount;
        RevenueAccountId = revenueAccountId;
        CostCenterId = costCenterId;
    }
}