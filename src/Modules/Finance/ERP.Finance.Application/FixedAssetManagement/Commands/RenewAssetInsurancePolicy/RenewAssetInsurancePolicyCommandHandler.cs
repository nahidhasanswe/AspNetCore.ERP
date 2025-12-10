using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RenewAssetInsurancePolicy;

public class RenewAssetInsurancePolicyCommandHandler(
    IAssetInsurancePolicyRepository policyRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RenewAssetInsurancePolicyCommand, Result>
{
    public async Task<Result> Handle(RenewAssetInsurancePolicyCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var policy = await policyRepository.GetByIdAsync(command.PolicyId, cancellationToken);
        if (policy == null)
        {
            return Result.Failure("Asset Insurance Policy not found.");
        }

        if (policy.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Asset Insurance Policy does not belong to the specified Business Unit.");
        }

        policy.Renew(
            command.NewEndDate,
            command.NewCoverageAmount,
            command.NewPremiumAmount
        );

        await policyRepository.UpdateAsync(policy, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}