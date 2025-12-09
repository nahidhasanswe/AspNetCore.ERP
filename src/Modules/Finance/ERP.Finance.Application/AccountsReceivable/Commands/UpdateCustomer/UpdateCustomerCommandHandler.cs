using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var customer = await customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure("Customer not found.");
        }
        
        var address = new Address(command.BillingAddress.Street, command.BillingAddress.City, command.BillingAddress.State, command.BillingAddress.Country, command.BillingAddress.PostalCode);
        var contactInfo = new ContactInfo(command.ContactInfo.Phone, command.ContactInfo.Email);

        customer.Update(
            command.Name,
            command.ContactEmail,
            address,
            contactInfo,
            command.PaymentTerms,
            command.DefaultCurrency,
            command.ARControlAccountId
        );

        await customerRepository.UpdateAsync(customer, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}