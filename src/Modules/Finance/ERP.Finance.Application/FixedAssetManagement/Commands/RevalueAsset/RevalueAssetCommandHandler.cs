using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RevalueAsset;

public class RevalueAssetCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RevalueAssetCommand, Result>
{
    public async Task<Result> Handle(RevalueAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fixedAsset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        if (fixedAsset.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Fixed Asset does not belong to the specified Business Unit.");
        }

        fixedAsset.Revalue(
            command.RevaluationDate,
            command.NewAcquisitionCost,
            command.RevaluationGainLossAccountId
        );

        await fixedAssetRepository.UpdateAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}