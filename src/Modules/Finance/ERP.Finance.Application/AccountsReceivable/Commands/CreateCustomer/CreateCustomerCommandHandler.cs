using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerCreditProfileRepository creditProfileRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        
        var address = new Address(command.BillingAddress.Street, command.BillingAddress.City, command.BillingAddress.State, command.BillingAddress.Country, command.BillingAddress.PostalCode);
        var contactInfo = new ContactInfo(command.ContactInfo.Phone, command.ContactInfo.Email);
        
        // 1. Create Customer (without credit profile ID initially)
        var customer = Customer.Create(
            command.Name,
            command.ContactEmail,
            address,
            contactInfo,
            command.PaymentTerms,
            command.DefaultCurrency,
            command.ARControlAccountId
        );

        // 2. Create CustomerCreditProfile using the newly created customer's ID
        var creditProfile = new CustomerCreditProfile(
            customer.Id, // Use the newly created customer's ID
            command.ApprovedCreditLimit,
            command.PaymentTerms
        );

        // 3. Assign the credit profile ID to the customer
        customer.AssignCreditProfile(creditProfile.Id);

        await customerRepository.AddAsync(customer, cancellationToken);
        await creditProfileRepository.AddAsync(creditProfile, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}