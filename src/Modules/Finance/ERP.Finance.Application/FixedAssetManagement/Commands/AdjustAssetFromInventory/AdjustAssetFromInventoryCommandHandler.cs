using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.AdjustAssetFromInventory;

public class AdjustAssetFromInventoryCommandHandler(
    IPhysicalInventoryRecordRepository recordRepository,
    IFixedAssetRepository fixedAssetRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<AdjustAssetFromInventoryCommand, Result>
{
    public async Task<Result> Handle(AdjustAssetFromInventoryCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var record = await recordRepository.GetByIdAsync(command.RecordId, cancellationToken);
        if (record == null)
        {
            return Result.Failure("Physical Inventory Record not found.");
        }

        var asset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (asset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        if (record.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Physical Inventory Record does not belong to the specified Business Unit.");
        }
        if (asset.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Fixed Asset does not belong to the specified Business Unit.");
        }
        if (record.AssetId != asset.Id)
        {
            return Result.Failure("Physical Inventory Record does not match the specified Asset.");
        }

        // Update asset's location
        asset.Update(
            asset.Description,
            asset.AssetAccountId,
            asset.DepreciationExpenseAccountId,
            asset.AccumulatedDepreciationAccountId,
            asset.CostCenterId,
            command.NewLocation
        );

        // Mark inventory record as adjusted
        record.MarkAsAdjusted();

        await recordRepository.UpdateAsync(record, cancellationToken);
        await fixedAssetRepository.UpdateAsync(asset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}