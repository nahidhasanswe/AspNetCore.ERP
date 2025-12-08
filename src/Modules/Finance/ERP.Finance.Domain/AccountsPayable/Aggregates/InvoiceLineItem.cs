using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class InvoiceLineItem : Entity
{
    public string Description { get; private set; }
    public Money LineAmount { get; private set; }
    public Guid ExpenseAccountId { get; private set; } // GL Account ID for the expense
    
    public Guid? CostCenterId { get; private set; }
    
    private InvoiceLineItem() { }
    
    public InvoiceLineItem(string description, Money lineAmount, Guid expenseAccountId, Guid? costCenterId) : base(Guid.NewGuid())
    {
        Description = description;
        LineAmount = lineAmount;
        ExpenseAccountId = expenseAccountId;
        CostCenterId = costCenterId;
    }
}