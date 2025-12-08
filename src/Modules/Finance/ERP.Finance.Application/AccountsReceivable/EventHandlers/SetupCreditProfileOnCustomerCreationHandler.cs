using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.EventHandlers;

public class SetupCreditProfileOnCustomerCreationHandler(
    ICustomerCreditProfileRepository creditRepository,
    IUnitOfWorkManager unitOfWork
) : INotificationHandler<CustomerCreatedEvent>
{
    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Define a default credit limit (e.g., $5000 USD)
        var defaultLimit = new Money(5000m, SystemBaseCurrency); 
        
        // Create the CustomerCreditProfile Aggregate
        var profile = new CustomerCreditProfile(notification.CustomerId, defaultLimit);

        using var scope = unitOfWork.Begin();
        
        // Persist the new credit profile for future checks
        await creditRepository.AddAsync(profile, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}