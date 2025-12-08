using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.AccountsReceivable.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class Customer : AggregateRoot
{
    public string Name { get; private set; }
    public string ContactEmail { get; private set; }
    public CustomerStatus Status { get; private set; }
    
    private Customer() { }

    private Customer(string name, string contactEmail) : base(Guid.NewGuid())
    {
        Name = name;
        ContactEmail = contactEmail;
        Status = CustomerStatus.PendingApproval;
        
        AddDomainEvent(new CustomerCreatedEvent(this.Id, name, contactEmail));
    }
    
    public static Customer Create(string name, string contactEmail)
    {
        var create = new Customer(name, contactEmail);
        
        return create;
    }

    public Result Approve()
    {
        if (Status != CustomerStatus.PendingApproval)
            // Correct failure message for state transition
            return Result.Failure($"Customer status is {Status}. Only PendingApproval can be approved.");
        
        Status = CustomerStatus.Active;
        return Result.Success();
    }
    
    public Result Active()
    {
        if (Status == CustomerStatus.Active)
            return Result.Failure("Customer is already active.");

        if (Status != CustomerStatus.Inactive)
            return Result.Failure($"Customer status is {Status}. Only Inactive customers can be manually set to Active.");
    
        Status = CustomerStatus.Active;
        return Result.Success();
    }
    
    public Result Inactive()
    {
        if (Status == CustomerStatus.Inactive)
            return Result.Failure("Customer is already inactive.");
        
        if (Status == CustomerStatus.PendingApproval)
            return Result.Failure("Cannot inactivate a customer that is Pending Approval. Must be Approved or Rejected first.");
    
        // FIX: Set status to Inactive
        Status = CustomerStatus.Inactive; 
        
        return Result.Success();
    }
}