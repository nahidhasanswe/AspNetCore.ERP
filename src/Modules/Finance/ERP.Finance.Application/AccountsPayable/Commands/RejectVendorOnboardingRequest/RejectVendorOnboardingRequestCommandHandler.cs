using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.RejectVendorOnboardingRequest;

public class RejectVendorOnboardingRequestCommandHandler(
    IVendorOnboardingRequestRepository onboardingRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<RejectVendorOnboardingRequestCommand, Unit>
{
    public async Task<Result<Unit>> Handle(RejectVendorOnboardingRequestCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var request = await onboardingRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure<Unit>("Vendor onboarding request not found.");
        }

        request.Reject(command.Reason);

        await onboardingRepository.UpdateAsync(request, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}