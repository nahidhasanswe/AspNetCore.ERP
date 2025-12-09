using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerCreditProfileRepository creditProfileRepository, // Added to constructor
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateCustomerCommand, Result>
{
    // Injected
    // Assigned

    public async Task<Result> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var customer = await customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure("Customer not found.");
        }

        // Load the associated credit profile
        if (!customer.CustomerCreditProfileId.HasValue)
        {
            return Result.Failure("Customer does not have an associated credit profile.");
        }
        var creditProfile = await creditProfileRepository.GetByIdAsync(customer.CustomerCreditProfileId.Value, cancellationToken);
        if (creditProfile == null)
        {
            return Result.Failure("Customer Credit Profile not found.");
        }
        
        var address = new Address(command.BillingAddress.Street, command.BillingAddress.City, command.BillingAddress.State, command.BillingAddress.Country, command.BillingAddress.PostalCode);
        var contactInfo = new ContactInfo(command.ContactInfo.Phone, command.ContactInfo.Email);

        // Update Customer details
        customer.Update(
            command.Name,
            command.ContactEmail,
            address,
            contactInfo,
            command.PaymentTerms,
            command.DefaultCurrency,
            command.ARControlAccountId
        );

        // Update Credit Profile details
        creditProfile.UpdateLimitAndTerms(command.ApprovedCreditLimit, command.PaymentTerms);

        await customerRepository.UpdateAsync(customer, cancellationToken);
        await creditProfileRepository.UpdateAsync(creditProfile, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}