using ERP.Core.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CustomerCreditProfile : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Money ApprovedLimit { get; private set; }
    public decimal CurrentExposure { get; private set; } // Total outstanding AR balance
    public CreditStatus Status { get; private set; }

    private CustomerCreditProfile() { }

    public CustomerCreditProfile(Guid customerId, Money initialLimit) : base(Guid.NewGuid())
    {
        CustomerId = customerId;
        ApprovedLimit = initialLimit;
        CurrentExposure = 0m;
        Status = CreditStatus.Active;
    }

    /// <summary>
    /// Checks if a new order amount would violate the approved credit limit.
    /// </summary>
    public bool CanApproveOrder(Money orderAmount)
    {
        if (Status != CreditStatus.Active) return false;

        // Note: Currency conversion would be needed here if orderAmount is not in ApprovedLimit.Currency
        var newExposure = CurrentExposure + orderAmount.Amount;
        
        return newExposure <= ApprovedLimit.Amount;
    }

    /// <summary>
    /// Increases the current exposure when a new invoice is issued.
    /// </summary>
    public void IncreaseExposure(Money invoiceAmount)
    {
        // Validation ensures the increase doesn't violate the limit (often checked by the calling service)
        CurrentExposure += invoiceAmount.Amount;
        // Optionally raise a CreditExposureIncreasedEvent
    }

    /// <summary>
    /// Decreases the current exposure when an invoice is fully or partially paid.
    /// </summary>
    public void DecreaseExposure(Money paymentAmount)
    {
        CurrentExposure -= paymentAmount.Amount;
        // Ensure exposure does not drop below zero
        CurrentExposure = Math.Max(0, CurrentExposure); 
    }

    public void PlaceCreditHold(string reason)
    {
        if (Status != CreditStatus.Active) return;
        Status = CreditStatus.OnHold;
        
        AddDomainEvent(new CustomerCreditHoldPlacedEvent(
            CustomerId,
            this.Id,
            reason
        ));
    }

    public void ReleaseCreditHold()
    {
        if (Status != CreditStatus.OnHold) return;
        Status = CreditStatus.Active;
        
        AddDomainEvent(new CustomerCreditHoldReleasedEvent(
            CustomerId,
            this.Id
        ));
    }
}