using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.PlaceCustomerCreditHold;

public class PlaceCustomerCreditHoldCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerCreditProfileRepository creditProfileRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<PlaceCustomerCreditHoldCommand, Result>
{
    public async Task<Result> Handle(PlaceCustomerCreditHoldCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var customer = await customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure("Customer not found.");
        }
        if (!customer.CustomerCreditProfileId.HasValue)
        {
            return Result.Failure("Customer does not have an associated credit profile.");
        }

        var creditProfile = await creditProfileRepository.GetByIdAsync(customer.CustomerCreditProfileId.Value, cancellationToken);
        if (creditProfile == null)
        {
            return Result.Failure("Customer Credit Profile not found.");
        }

        creditProfile.PlaceCreditHold(command.Reason);

        await creditProfileRepository.UpdateAsync(creditProfile, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}