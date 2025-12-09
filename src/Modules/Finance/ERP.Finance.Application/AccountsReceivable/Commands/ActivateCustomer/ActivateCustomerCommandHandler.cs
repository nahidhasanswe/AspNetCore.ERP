using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ActivateCustomer;

public class ActivateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ActivateCustomerCommand, Result>
{
    public async Task<Result> Handle(ActivateCustomerCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var customer = await customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure("Customer not found.");
        }

        customer.Activate();

        await customerRepository.UpdateAsync(customer, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}