using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RetireFixedAsset;

public class RetireFixedAssetCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RetireFixedAssetCommand, Result>
{
    public async Task<Result> Handle(RetireFixedAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fixedAsset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        fixedAsset.Retire(
            command.RetirementDate,
            command.Reason
        );

        await fixedAssetRepository.UpdateAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}