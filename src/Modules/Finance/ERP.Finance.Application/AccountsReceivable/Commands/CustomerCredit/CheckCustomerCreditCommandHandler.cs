using ERP.Core;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CustomerCredit;

public class CheckCustomerCreditCommandHandler(
    ICustomerCreditProfileRepository repository
) : IRequestHandler<CheckCustomerCreditCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckCustomerCreditCommand command, CancellationToken cancellationToken)
    {
        // 1. Retrieve the Credit Profile Aggregate
        var profile = await repository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);

        if (profile == null)
        {
            // Default policy: If no profile exists, assume credit is required but missing configuration.
            return Result.Failure<bool>($"Credit profile not found for Customer {command.CustomerId}. Order blocked.");
        }

        // 2. Execute Domain Logic: Check invariant
        if (profile.CanApproveOrder(command.OrderAmount))
        {
            // If approved, the Order Management system can proceed.
            return Result.Success(true);
        }
        else
        {
            // Failure: Credit limit exceeded or customer is on hold.
            // A CustomerCreditHoldPlacedEvent should have already been raised if the status is OnHold.
            return Result.Failure<bool>($"Credit check failed. Current exposure ({profile.CurrentExposure}) + order amount exceeds limit ({profile.ApprovedLimit.Amount}).");
        }
    }
}