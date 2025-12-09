using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.UpdateFixedAsset;

public class UpdateFixedAssetCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateFixedAssetCommand, Result>
{
    public async Task<Result> Handle(UpdateFixedAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fixedAsset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        fixedAsset.Update(
            command.Description,
            command.AssetAccountId,
            command.DepreciationExpenseAccountId,
            command.AccumulatedDepreciationAccountId,
            command.CostCenterId,
            command.Location
        );

        await fixedAssetRepository.UpdateAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}