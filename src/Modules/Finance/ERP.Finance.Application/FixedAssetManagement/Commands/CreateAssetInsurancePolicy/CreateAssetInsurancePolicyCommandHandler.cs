using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateAssetInsurancePolicy;

public class CreateAssetInsurancePolicyCommandHandler : IRequestCommandHandler<CreateAssetInsurancePolicyCommand, Guid>
{
    private readonly IAssetInsurancePolicyRepository _policyRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateAssetInsurancePolicyCommandHandler(IAssetInsurancePolicyRepository policyRepository, IUnitOfWorkManager unitOfWork)
    {
        _policyRepository = policyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAssetInsurancePolicyCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var policy = new AssetInsurancePolicy(
            command.AssetId,
            command.PolicyNumber,
            command.Insurer,
            command.StartDate,
            command.EndDate,
            command.CoverageAmount,
            command.PremiumAmount
        );

        await _policyRepository.AddAsync(policy, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(policy.Id);
    }
}