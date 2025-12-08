using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    ICustomerRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        // 1. Input Validation (Beyond simple null/whitespace checks)
        // Example: Check if a customer with this email already exists (invariant check)
        var existingCustomer = await repository.GetByEmailAsync(command.ContactEmail, cancellationToken);
        if (existingCustomer is not null)
        {
            return Result.Failure<Guid>($"A customer with email {command.ContactEmail} already exists.");
        }

        // 2. Execute Domain Logic: Create the Aggregate Root
        // The Aggregate constructor validates and raises the CustomerCreatedEvent.
        Customer customer;
        try
        {
            customer = Customer.Create(command.Name, command.ContactEmail);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }

        // 3. Persist and Dispatch Event
        // The UnitOfWork ensures the customer is saved and the event is dispatched atomically.
        using var scope = unitOfWork.Begin();
        await repository.AddAsync(customer, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(customer.Id);
    }
}