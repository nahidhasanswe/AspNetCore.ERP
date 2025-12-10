using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateAssetInsurancePolicy;

public class CreateAssetInsurancePolicyCommandHandler(
    IAssetInsurancePolicyRepository policyRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateAssetInsurancePolicyCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAssetInsurancePolicyCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var policy = new AssetInsurancePolicy(
            command.BusinessUnitId,
            command.AssetId,
            command.PolicyNumber,
            command.Insurer,
            command.StartDate,
            command.EndDate,
            command.CoverageAmount,
            command.PremiumAmount
        );

        await policyRepository.AddAsync(policy, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(policy.Id);
    }
}