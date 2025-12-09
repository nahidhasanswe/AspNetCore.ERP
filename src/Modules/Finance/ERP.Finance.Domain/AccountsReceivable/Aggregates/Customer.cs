using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class Customer : AggregateRoot
{
    public string Name { get; private set; }
    public string ContactEmail { get; private set; }
    public Address BillingAddress { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public string PaymentTerms { get; private set; }
    public string DefaultCurrency { get; private set; }
    public Guid ARControlAccountId { get; private set; }
    public CustomerStatus Status { get; private set; }
    public Guid? CustomerCreditProfileId { get; private set; } // New property

    private Customer() { }

    private Customer(
        string name, 
        string contactEmail, 
        Address billingAddress, 
        ContactInfo contactInfo, 
        string paymentTerms, 
        string defaultCurrency, 
        Guid arControlAccountId,
        Guid? customerCreditProfileId) : base(Guid.NewGuid())
    {
        Name = name;
        ContactEmail = contactEmail;
        BillingAddress = billingAddress;
        ContactInfo = contactInfo;
        PaymentTerms = paymentTerms;
        DefaultCurrency = defaultCurrency;
        ARControlAccountId = arControlAccountId;
        Status = CustomerStatus.PendingApproval;
        CustomerCreditProfileId = customerCreditProfileId;
        
        AddDomainEvent(new CustomerCreatedEvent(this.Id, name, contactEmail));
    }
    
    public static Customer Create(
        string name, 
        string contactEmail, 
        Address billingAddress, 
        ContactInfo contactInfo, 
        string paymentTerms, 
        string defaultCurrency, 
        Guid arControlAccountId,
        Guid? customerCreditProfileId = null)
    {
        return new Customer(name, contactEmail, billingAddress, contactInfo, paymentTerms, defaultCurrency, arControlAccountId, customerCreditProfileId);
    }

    public void AssignCreditProfile(Guid creditProfileId)
    {
        if (CustomerCreditProfileId.HasValue)
            throw new InvalidOperationException("Customer already has a credit profile assigned.");
        CustomerCreditProfileId = creditProfileId;
    }

    public Result Update(
        string name, 
        string contactEmail, 
        Address billingAddress, 
        ContactInfo contactInfo, 
        string paymentTerms, 
        string defaultCurrency, 
        Guid arControlAccountId)
    {
        if (Status == CustomerStatus.PendingApproval)
            return Result.Failure("Cannot update customer details while in Pending Approval status.");

        Name = name;
        ContactEmail = contactEmail;
        BillingAddress = billingAddress;
        ContactInfo = contactInfo;
        PaymentTerms = paymentTerms;
        DefaultCurrency = defaultCurrency;
        ARControlAccountId = arControlAccountId;
        
        return Result.Success();
    }

    public Result Approve()
    {
        if (Status != CustomerStatus.PendingApproval)
            return Result.Failure($"Customer status is {Status}. Only PendingApproval can be approved.");
        
        Status = CustomerStatus.Active;
        return Result.Success();
    }
    
    public Result Activate()
    {
        if (Status == CustomerStatus.Active)
            return Result.Failure("Customer is already active.");

        if (Status != CustomerStatus.Inactive)
            return Result.Failure($"Customer status is {Status}. Only Inactive customers can be manually activated.");
    
        Status = CustomerStatus.Active;
        return Result.Success();
    }
    
    public Result Deactivate()
    {
        if (Status == CustomerStatus.Inactive)
            return Result.Failure("Customer is already inactive.");
        
        if (Status == CustomerStatus.PendingApproval)
            return Result.Failure("Cannot deactivate a customer that is Pending Approval. Must be Approved or Rejected first.");
    
        Status = CustomerStatus.Inactive; 
        
        return Result.Success();
    }
}