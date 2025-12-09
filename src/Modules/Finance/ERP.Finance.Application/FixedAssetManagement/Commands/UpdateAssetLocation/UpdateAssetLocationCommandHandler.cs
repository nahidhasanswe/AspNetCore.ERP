using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.UpdateAssetLocation;

public class UpdateAssetLocationCommandHandler(
    IFixedAssetRepository fixedAssetRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateAssetLocationCommand, Result>
{
    public async Task<Result> Handle(UpdateAssetLocationCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fixedAsset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        // Assuming the Update method in FixedAsset can handle location updates
        fixedAsset.Update(
            fixedAsset.Description, // Keep existing description
            fixedAsset.AssetAccountId, // Keep existing account
            fixedAsset.DepreciationExpenseAccountId, // Keep existing account
            fixedAsset.AccumulatedDepreciationAccountId, // Keep existing account
            fixedAsset.CostCenterId, // Keep existing cost center
            command.NewLocation // Update location
        );

        await fixedAssetRepository.UpdateAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}